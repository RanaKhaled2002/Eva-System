using Eva_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();
        IGenericRepository<T> Repository<T>() where T : BaseClass;
    }
}
