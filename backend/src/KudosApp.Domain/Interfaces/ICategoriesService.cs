using KudosApp.Domain.DTOs.Categories;

namespace KudosApp.Domain.Interfaces;

public interface ICategoriesService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync();
}
