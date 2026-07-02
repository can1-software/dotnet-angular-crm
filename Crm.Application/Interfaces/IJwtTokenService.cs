using Crm.Domain.Entities;

namespace Crm.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
}
