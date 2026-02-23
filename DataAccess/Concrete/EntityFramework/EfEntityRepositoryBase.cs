using Core.DataAccess;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfEntityRepositoryBase<TContext, TEntity> : IEntityRepository<TEntity>
        where TContext : DbContext, new()
        where TEntity : class, IEntity, new()
    {
        private readonly TContext context;
        public EfEntityRepositoryBase(TContext _context)
        {
            context = _context;
        }


        public async Task AddSync(TEntity entity, CancellationToken ct = default)
        {
            await context.Set<TEntity>().AddAsync(entity, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            context.Set<TEntity>().Update(entity);
            await context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken ct = default)
        {
            return filter == null
                ? await context.Set<TEntity>().ToListAsync(ct)
                : await context.Set<TEntity>().Where(filter).ToListAsync(ct);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
        {
            return await context.Set<TEntity>().FirstOrDefaultAsync(filter, ct);
        }
    }
}
