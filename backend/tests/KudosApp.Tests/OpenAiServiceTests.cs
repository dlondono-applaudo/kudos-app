using KudosApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace KudosApp.Tests;

public class OpenAiServiceTests
{
    private static OpenAiService CreateServiceWithoutKey()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenAI:ApiKey"] = "",
                ["OpenAI:Model"] = "gpt-4o-mini"
            })
            .Build();

        return new OpenAiService(config, NullLogger<OpenAiService>.Instance);
    }

    [Fact]
    public async Task SuggestMessages_WithoutApiKey_Returns3Fallbacks()
    {
        var service = CreateServiceWithoutKey();

        var suggestions = await service.SuggestMessagesAsync("Teamwork", "helped debug");

        Assert.Equal(3, suggestions.Count);
        Assert.All(suggestions, s => Assert.False(string.IsNullOrWhiteSpace(s)));
    }

    [Fact]
    public async Task SuggestMessages_WithoutApiKey_IncludesIntentInMessages()
    {
        var service = CreateServiceWithoutKey();

        var suggestions = await service.SuggestMessagesAsync("Innovation", "created new tool");

        Assert.All(suggestions, s => Assert.Contains("created new tool", s));
    }

    [Fact]
    public async Task ValidateAndAnalyze_WithoutApiKey_AutoApproves()
    {
        var service = CreateServiceWithoutKey();

        var result = await service.ValidateAndAnalyzeAsync("Great teamwork!", "Teamwork");

        Assert.True(result.IsApproved);
        Assert.Null(result.Reason);
    }

    [Fact]
    public async Task ValidateAndAnalyze_WithoutApiKey_ReturnsDefaultEmoji()
    {
        var service = CreateServiceWithoutKey();

        var result = await service.ValidateAndAnalyzeAsync("Awesome work!", "Leadership");

        Assert.Equal("🌟", result.SentimentEmoji);
    }
}
