using Core.Entities;
using System.Linq.Expressions;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);

        Task AddSync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
    }
}
