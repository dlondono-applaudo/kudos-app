using KudosApp.Core.DTOs.Categories;
using KudosApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll()
    {
        var categories = await _context.Categories
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.PointValue))
            .ToListAsync();

        return Ok(categories);
    }
}
