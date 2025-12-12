using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CardGame.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Api.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly CardGameContext? _context;
        protected readonly DbSet<T>? _set;

        public EfRepository(CardGameContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await _set!.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
            => await _set!.AddRangeAsync(entities, ct);

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _set!.FindAsync(new object[] { id }, ct).AsTask();

        public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default)
            => await _set!.ToListAsync(ct);

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _set!.Where(predicate).ToListAsync(ct);

        public void Remove(T entity)
        {
            _set!.Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context!.SaveChangesAsync(ct);
        }
    }
}