using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace Grupo13Fiap.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
       where T : EntityBase
    {
        protected readonly DbSet<T> Entities;
        protected readonly DBContextGrupo13Fiap Context;

        public GenericRepository(DBContextGrupo13Fiap context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Entities = Context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public virtual async Task<IEnumerable<T>> GetAllAsync(bool tracking = false, CancellationToken cancellationToken = default)
            => await Query(tracking)
                .ToListAsync(cancellationToken);

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await Query()
                .Where(predicate)
                .ToListAsync(cancellationToken);

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            SetCreateDate(entity);
            await Entities.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var entitiesArray = entities.ToArray();
            SetCreateDate(entitiesArray);
            await Entities.AddRangeAsync(entitiesArray, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            Entities.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var entitiesArray = entities.ToArray();
            Entities.UpdateRange(entitiesArray);
            await Context.SaveChangesAsync(cancellationToken);
        }

        private void SetCreateDate(params T[] entities)
        {
            var now = DateTime.UtcNow;

            foreach(var item in entities)
                item.CreateDate = now;
        }

        protected IQueryable<T> Query(bool tracking = false)
            => tracking
            ? Entities
            : Entities.AsNoTracking();

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
            => await Query()
                .Where(e => e.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
    }
}
