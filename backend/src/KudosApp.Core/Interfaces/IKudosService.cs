using KudosApp.Core.DTOs.Kudos;

namespace KudosApp.Core.Interfaces;

public interface IKudosService
{
    Task<KudosResponse> CreateAsync(string senderId, CreateKudosRequest request);
    Task<KudosFeedResponse> GetFeedAsync(int page, int pageSize);
    Task<KudosResponse?> GetByIdAsync(int id);
    Task<IReadOnlyList<KudosResponse>> GetBySenderAsync(string userId);
    Task<IReadOnlyList<KudosResponse>> GetByReceiverAsync(string userId);
    Task<bool> DeleteAsync(int id);
}
