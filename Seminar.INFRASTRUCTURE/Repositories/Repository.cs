using Microsoft.EntityFrameworkCore;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.INFRASTRUCTURE.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly SeminarContext _context;

        public IQueryable<T> Entities => _context.Set<T>();

        public Repository(SeminarContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new Exception("Entity not found");
            _dbSet.Remove(entity);
        }
    }
}
