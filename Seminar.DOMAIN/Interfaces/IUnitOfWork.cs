using Microsoft.EntityFrameworkCore.Storage;
namespace Seminar.DOMAIN.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task SaveChangesAsync();
        IExecutionStrategy CreateExecutionStrategy();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackAsync();
    }
}
