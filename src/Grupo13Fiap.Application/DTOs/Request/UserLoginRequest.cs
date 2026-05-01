using System.ComponentModel.DataAnnotations;

namespace Grupo13Fiap.Application.DTOs.Request;

public class UserLoginRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Password { get; set; } = string.Empty;
}