using System.Linq.Expressions;

namespace CardGame.Api.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);

        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        void Remove(T entity);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}