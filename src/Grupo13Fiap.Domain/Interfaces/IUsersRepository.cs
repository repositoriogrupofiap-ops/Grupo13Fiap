using Grupo13Fiap.Domain.Entities;

namespace Grupo13Fiap.Domain.Interfaces;

public interface IUsersRepository : IGenericRepository<User>
{
    Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<User?> GetWithLibraryAsync(Guid id, CancellationToken cancellationToken = default);
}
