using System.ComponentModel.DataAnnotations;

namespace KudosApp.Core.DTOs.Auth;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MaxLength(100)] string FullName,
    [Required, MaxLength(100)] string Department
);
