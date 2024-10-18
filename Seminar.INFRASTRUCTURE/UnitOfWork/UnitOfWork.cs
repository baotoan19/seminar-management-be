using Microsoft.EntityFrameworkCore.Storage;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Database;
using Seminar.INFRASTRUCTURE.Repositories;

namespace Seminar.INFRASTRUCTURE.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SeminarContext _context;
        private bool _disposed = false;

        public UnitOfWork(SeminarContext context)
        {
            _context = context;
        }
        
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollBackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
        }

    }
}