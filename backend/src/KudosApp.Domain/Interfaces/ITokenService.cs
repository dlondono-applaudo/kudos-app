using KudosApp.Domain.Entities;

namespace KudosApp.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
