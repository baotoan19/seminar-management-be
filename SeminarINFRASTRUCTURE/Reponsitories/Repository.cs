using Seminar.DOMAIN.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeminarINFRASTRUCTURE.Reponsitories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> dbSet;

    }
}
