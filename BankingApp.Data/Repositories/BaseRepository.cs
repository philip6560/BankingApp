using BankingApp.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly BankingAppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(BankingAppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }


        public virtual Task UpdateAsync(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await GetByIdAsync(id);
            await DeleteAsync(entityToDelete);
        }

        public virtual Task DeleteAsync(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            return Task.CompletedTask;
        }
    }
}
