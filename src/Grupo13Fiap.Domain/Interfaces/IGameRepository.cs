using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;

namespace Grupo13Fiap.Domain.Interfaces;

public interface IGameRepository : IGenericRepository<Game>
{
    Task<IEnumerable<Game>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetByCategoryAsync(CategoryGameEnum category, CancellationToken cancellationToken = default);
}
