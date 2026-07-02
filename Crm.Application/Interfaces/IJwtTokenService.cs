using Crm.Application.DTOs.Auth;
using Crm.Domain.Entities;

namespace Crm.Application.Interfaces;

public interface IJwtTokenService
{
    AccessTokenResult GenerateAccessToken(AppUser user, IList<string> roles);
}
