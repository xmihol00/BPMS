using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BPMS_DAL.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly BpmsDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(BpmsDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public Task<int> Save()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<EntityEntry<T>> Create(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public async Task CreateRange(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IDbContextTransaction> CreateTransaction()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Entry<U>(U entity, Action<EntityEntry<U>> action) where U : class
        {
            action(_context.Entry(entity));
        }
    }
}
