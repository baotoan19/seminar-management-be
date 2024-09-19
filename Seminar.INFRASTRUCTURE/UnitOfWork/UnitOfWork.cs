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