using Microsoft.AspNetCore.Identity;

namespace KudosApp.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // EF Core needs a parameterless constructor
    private ApplicationUser() { }

    public static ApplicationUser Create(string email, string fullName, string department)
    {
        return new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            Department = department,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string fullName, string department, string? avatarUrl = null)
    {
        FullName = fullName;
        Department = department;
        if (avatarUrl is not null)
            AvatarUrl = avatarUrl;
    }
}
