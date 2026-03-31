using Grupo13Fiap.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Grupo13Fiap.Application.DTOs.Request;

public class GameRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [MaxLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "O campo {0} deve ser maior ou igual a zero")]
    public decimal Price { get; set; }

    public CategoryGameEnum Category { get; set; }

    public DateTime? DisponibilizationDate { get; set; }
}