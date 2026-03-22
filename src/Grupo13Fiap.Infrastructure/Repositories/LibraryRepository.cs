using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Repositories;

public class LibraryRepository(DBContextGrupo13Fiap context) : GenericRepository<Library>(context), ILibraryRepository
{
    public async Task<Library?> GetWithGamesAsync(Guid id, CancellationToken cancellationToken = default)
        => await Query()
            .Include(l => l.Games)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
}
