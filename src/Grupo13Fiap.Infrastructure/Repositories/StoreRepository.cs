using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Repositories;

public class StoreRepository(DBContextGrupo13Fiap context) : GenericRepository<Store>(context), IStoreRepository
{
    public async Task<Store?> GetWithGamesAsync(Guid id, CancellationToken cancellationToken = default)
        => await Query()
            .Include(s => s.Games)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
