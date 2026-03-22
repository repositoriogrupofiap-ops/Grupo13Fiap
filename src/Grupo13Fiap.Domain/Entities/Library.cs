namespace Grupo13Fiap.Domain.Entities;

public class Library : EntityBase
{
    public ICollection<Game> Games { get; set; } = [];
}
