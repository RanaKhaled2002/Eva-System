using Eva_BLL.Interfaces;
using Eva_DAL.Data;
using Eva_DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseClass
    {
        private readonly EvaDbContext _evaDbContext;

        public GenericRepository(EvaDbContext evaDbContext)
        {
            _evaDbContext = evaDbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
             Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _evaDbContext.Set<T>();

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }


        public async Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _evaDbContext.Set<T>();

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(T entity)
        {
            await _evaDbContext.AddAsync(entity);
        }

        public async Task Update(T entity)
        {
            _evaDbContext.Update(entity);
        }

        public async Task Delete(T entity)
        {
            _evaDbContext.Remove(entity);
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _evaDbContext.Set<T>();

            if (include != null)
            {
                query = include(query);
            }

            return query.FirstOrDefaultAsync(filter);
        }
    }
}
