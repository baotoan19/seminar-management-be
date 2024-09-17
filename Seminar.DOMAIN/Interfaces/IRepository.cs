using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<IList<T>> GetAll();
        Task<T> GetById(int id);
        Task Insert(T entity);
        Task Delete(int id);
        Task Update(T entity);

    }
}
