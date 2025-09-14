using Eva_BLL.Interfaces;
using Eva_DAL.Data;
using Eva_DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EvaDbContext _evaDbContext;
        private Hashtable _repositories;


        public UnitOfWork(EvaDbContext evaDbContext)
        {
            _evaDbContext = evaDbContext;
            _repositories = new Hashtable();
        }

        public async Task<int> CompleteAsync()
        {
            return await _evaDbContext.SaveChangesAsync();
        }

        public IGenericRepository<T> Repository<T>() where T : BaseClass
        {
            // Ex: type = student
            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                // بينشئ نسخه جديده ويحقنه بال dbContext
                var repository = new GenericRepository<T>(_evaDbContext);

                _repositories.Add(type, repository);
            }

            return _repositories[type] as IGenericRepository<T>;
        }
    }
}
