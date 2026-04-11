using KudosApp.Domain.DTOs.Categories;
using KudosApp.Domain.Interfaces;
using KudosApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Application.Services;

public class CategoriesService : ICategoriesService
{
    private readonly AppDbContext _context;

    public CategoriesService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.PointValue))
            .ToListAsync();
    }
}
