namespace Grupo13Fiap.Domain.Entities;

public class Store : EntityBase
{
    public ICollection<Game> Games { get; set; } = [];
}
