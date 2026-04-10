using KudosApp.Core.Entities;

namespace KudosApp.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
