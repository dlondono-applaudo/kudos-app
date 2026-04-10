namespace KudosApp.Core.DTOs.Auth;

public record AuthResponse(
    string Token,
    DateTime Expiration,
    string Email,
    string FullName,
    IList<string> Roles
);
