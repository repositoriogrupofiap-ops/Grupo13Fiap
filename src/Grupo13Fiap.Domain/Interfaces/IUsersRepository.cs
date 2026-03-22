using Grupo13Fiap.Domain.Entities;

namespace Grupo13Fiap.Domain.Interfaces;

public interface IUsersRepository : IGenericRepository<Users>
{
    Task<Users?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Users?> GetWithLibraryAsync(Guid id, CancellationToken cancellationToken = default);
}
