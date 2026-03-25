using Grupo13Fiap.Domain.Enum;

namespace Grupo13Fiap.Domain.Entities;

public class User : EntityBase
{
    private readonly List<UserRoleEnum> _roles = [];

    public string Name { get; private set; } = string.Empty;
    public Guid? LibraryId { get; private set; }
    public Library? Library { get; private set; }

    public IReadOnlyCollection<UserRoleEnum> Roles => _roles.AsReadOnly();

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

    public void AddRole(UserRoleEnum role)
    {
        if (_roles.Contains(role))
            throw new InvalidOperationException("O usuário já possui esse perfil.");

        _roles.Add(role);
    }

    public void RemoveRole(UserRoleEnum role)
    {
        if (!_roles.Contains(role))
            throw new InvalidOperationException("O usuário não possui esse perfil.");

        _roles.Remove(role);
    }

    public bool HasRole(UserRoleEnum role)
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
        return HasRole(UserRoleEnum.Buyer);
    }

    public bool CanApproveGames()
    {
        return HasRole(UserRoleEnum.Admin);
    }

    public bool CanPublishGames()
    {
        return HasRole(UserRoleEnum.Publisher);
    }
}