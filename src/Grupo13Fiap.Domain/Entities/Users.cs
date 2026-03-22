namespace Grupo13Fiap.Domain.Entities;

public class Users : EntityBase
{
    public Guid? LibraryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Library? Library { get; set; }
}

