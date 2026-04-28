using System.ComponentModel.DataAnnotations;

namespace Grupo13Fiap.Application.DTOs.Request;

public class UserRegistrationRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password))]
    public string PasswordConfirmation { get; set; } = string.Empty;
}