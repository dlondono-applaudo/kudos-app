namespace KudosApp.Domain.DTOs.Auth;

public record AuthResponse(string Token, DateTime Expiration, string Email, string FullName, IList<string> Roles);
