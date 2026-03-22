using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Repositories;

public class GameRepository(DBContextGrupo13Fiap context) : GenericRepository<Game>(context), IGameRepository
{
    public async Task<IEnumerable<Game>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await Query()
            .Where(g => g.Nome.Contains(name))
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Game>> GetByCategoryAsync(CategoryGameEnum category, CancellationToken cancellationToken = default)
        => await Query()
            .Where(g => g.Category == category)
            .ToListAsync(cancellationToken);
}
