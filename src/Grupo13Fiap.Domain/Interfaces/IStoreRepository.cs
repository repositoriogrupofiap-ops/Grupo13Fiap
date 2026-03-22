using Grupo13Fiap.Domain.Entities;

namespace Grupo13Fiap.Domain.Interfaces;

public interface IStoreRepository : IGenericRepository<Store>
{
    Task<Store?> GetWithGamesAsync(Guid id, CancellationToken cancellationToken = default);
}
