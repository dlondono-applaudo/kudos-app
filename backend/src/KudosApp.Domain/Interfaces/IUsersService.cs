using KudosApp.Domain.DTOs.Users;

namespace KudosApp.Domain.Interfaces;

public interface IUsersService
{
    Task<UserProfileResponse?> GetProfileAsync(string userId);
    Task<IReadOnlyList<UserListItem>> GetAllAsync();
}
