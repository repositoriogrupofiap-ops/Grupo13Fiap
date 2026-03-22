using Grupo13Fiap.Domain.Entities;

namespace Grupo13Fiap.Domain.Interfaces;

public interface ILibraryRepository : IGenericRepository<Library>
{
    Task<Library?> GetWithGamesAsync(Guid id, CancellationToken cancellationToken = default);
}
