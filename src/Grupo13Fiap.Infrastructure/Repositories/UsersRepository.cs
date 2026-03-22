using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Repositories;

public class UsersRepository(DBContextGrupo13Fiap context) : GenericRepository<Users>(context), IUsersRepository
{
    public async Task<Users?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await Query().FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
    }

    public async Task<Users?> GetWithLibraryAsync(Guid id, CancellationToken cancellationToken = default)
        => await Query()
            .Include(u => u.Library)
                .ThenInclude(l => l.Games)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
}
