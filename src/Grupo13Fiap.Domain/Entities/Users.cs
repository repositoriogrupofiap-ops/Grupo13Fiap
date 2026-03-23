using Grupo13Fiap.Domain.Enum;

namespace Grupo13Fiap.Domain.Entities;

public class User : EntityBase
{
    private readonly List<UserRole> _roles = [];

    public string Name { get; private set; }
    public Guid? LibraryId { get; private set; }
    public Library? Library { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    public User(string name)
    {
        SetName(name);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio.");

        Name = name.Trim();
    }

    public void AddRole(UserRole role)
    {
        if (_roles.Contains(role))
            throw new InvalidOperationException("O usuário já possui esse perfil.");

        _roles.Add(role);
    }

    public void RemoveRole(UserRole role)
    {
        if (!_roles.Contains(role))
            throw new InvalidOperationException("O usuário não possui esse perfil.");

        _roles.Remove(role);
    }

    public bool HasRole(UserRole role)
    {
        return _roles.Contains(role);
    }

    public void AssignLibrary(Library library)
    {
        if (library is null)
            throw new ArgumentNullException(nameof(library));

        Library = library;
        LibraryId = library.Id;
    }

    public bool CanBuyGames()
    {
        return HasRole(UserRole.Buyer);
    }

    public bool CanApproveGames()
    {
        return HasRole(UserRole.Admin);
    }

    public bool CanPublishGames()
    {
        return HasRole(UserRole.Publisher);
    }
}