using Grupo13Fiap.Domain.Enum;

namespace Grupo13Fiap.Application.DTOs.Response;

public class GameResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CategoryGameEnum Category { get; set; }
    public DateTime DisponibilizationDate { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreateDate { get; set; }
}