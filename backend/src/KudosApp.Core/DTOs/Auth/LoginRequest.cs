using System.ComponentModel.DataAnnotations;

namespace KudosApp.Core.DTOs.Auth;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);
