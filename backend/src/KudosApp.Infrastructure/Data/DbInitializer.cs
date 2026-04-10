using KudosApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace KudosApp.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Created role {Role}", role);
            }
        }

        const string adminEmail = "admin@kudos.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = ApplicationUser.Create(adminEmail, "Admin User", "Management");
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                logger.LogInformation("Seeded admin user {Email}", adminEmail);
            }
        }

        // Seed a regular user for testing
        const string userEmail = "user@kudos.com";
        if (await userManager.FindByEmailAsync(userEmail) is null)
        {
            var user = ApplicationUser.Create(userEmail, "Test User", "Engineering");
            var result = await userManager.CreateAsync(user, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                logger.LogInformation("Seeded test user {Email}", userEmail);
            }
        }
    }
}
