namespace KudosApp.Core.DTOs.Categories;

public record CategoryResponse(
    int Id,
    string Name,
    string Description,
    int PointValue
);
