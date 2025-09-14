using Eva_DAL.DTOs;
using Eva_DAL.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Interfaces
{
    public interface IGenericRepository<T> where T : BaseClass
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
