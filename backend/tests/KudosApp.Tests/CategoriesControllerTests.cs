using System.Net;
using System.Net.Http.Json;
using KudosApp.Core.DTOs.Categories;

namespace KudosApp.Tests;

public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_AllowsAnonymous_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededCategories()
    {
        var categories = await _client.GetFromJsonAsync<List<CategoryResponse>>("/api/categories");

        Assert.NotNull(categories);
        Assert.Equal(5, categories.Count);
    }

    [Fact]
    public async Task GetAll_ContainsExpectedCategoryNames()
    {
        var categories = await _client.GetFromJsonAsync<List<CategoryResponse>>("/api/categories");

        Assert.NotNull(categories);
        Assert.Contains(categories, c => c.Name == "Teamwork");
        Assert.Contains(categories, c => c.Name == "Innovation");
        Assert.Contains(categories, c => c.Name == "Leadership");
        Assert.Contains(categories, c => c.Name == "Problem Solving");
        Assert.Contains(categories, c => c.Name == "Going Extra Mile");
    }

    [Fact]
    public async Task GetAll_CategoriesHavePositivePointValues()
    {
        var categories = await _client.GetFromJsonAsync<List<CategoryResponse>>("/api/categories");

        Assert.NotNull(categories);
        Assert.All(categories, c => Assert.True(c.PointValue > 0));
    }
}
