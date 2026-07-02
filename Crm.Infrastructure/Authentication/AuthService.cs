using Crm.Application.DTOs.Auth;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Crm.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
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

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt = tokenResult.ExpiresAt
        };
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
