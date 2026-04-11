using KudosApp.Domain.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace KudosApp.Api.Endpoints;

public static class CategoriesEndpoints
{
    public static IEndpointRouteBuilder MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/categories").WithTags("Categories");

        group.MapGet("", [OutputCache(Duration = 3600)] async (ICategoriesService categoriesService) =>
            Results.Ok(await categoriesService.GetAllAsync()));

        return app;
    }
}
