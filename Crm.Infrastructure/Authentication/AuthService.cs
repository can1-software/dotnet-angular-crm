using System.Security.Cryptography;
using Crm.Application.DTOs.Auth;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Crm.Persistence.Context;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Crm.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly CrmDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService,
        CrmDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ValidationException(MapIdentityErrors(result.Errors));
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var tokenResult = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = CreateRefreshToken(user.Id);

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt = tokenResult.ExpiresAt,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        storedToken.RevokedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var tokenResult = _jwtTokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = CreateRefreshToken(user.Id);

        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        return new RefreshTokenResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt = tokenResult.ExpiresAt,
            RefreshToken = newRefreshToken.Token
        };
    }

    public async Task LogoutAsync(LogoutRequest request)
    {
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (storedToken is not null && storedToken.IsActive)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || !user.IsActive)
            throw new KeyNotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };
    }

    private RefreshToken CreateRefreshToken(Guid userId)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        return new RefreshToken
        {
            Token = GenerateSecureToken(),
            UserId = userId,
            ExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString()
        };
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private static List<ValidationFailure> MapIdentityErrors(IEnumerable<IdentityError> errors)
    {
        return errors.Select(error => new ValidationFailure(
            error.Code switch
            {
                "DuplicateUserName" or "DuplicateEmail" => "Email",
                "PasswordTooShort" or "PasswordRequiresDigit" or "PasswordRequiresLower" or
                "PasswordRequiresUpper" or "PasswordRequiresNonAlphanumeric" => "Password",
                _ => error.Code
            },
            error.Description)).ToList();
    }
}
