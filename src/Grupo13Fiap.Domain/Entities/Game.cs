using Grupo13Fiap.Domain.Enum;

namespace Grupo13Fiap.Domain.Entities;

public class Game : EntityBase
{
    public CategoryGameEnum Category { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DisponibilizationDate { get; set; }
}
