using FluentValidation;
using KudosApp.Application.Services;
using KudosApp.Domain.Interfaces;
using KudosApp.Domain.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace KudosApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IKudosService, KudosService>();
        services.AddScoped<ICategoriesService, CategoriesService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<INotificationsService, NotificationsService>();
        services.AddScoped<IUsersService, UsersService>();

        services.AddSingleton<OpenAiService>();
        services.AddSingleton<IAiSuggestionService>(sp => sp.GetRequiredService<OpenAiService>());
        services.AddSingleton<IContentModerationService>(sp => sp.GetRequiredService<OpenAiService>());

        services.AddValidatorsFromAssemblyContaining<CreateKudosRequestValidator>();

        return services;
    }
}
