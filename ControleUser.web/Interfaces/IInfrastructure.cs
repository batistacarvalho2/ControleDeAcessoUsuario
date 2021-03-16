using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleUser.web.Interfaces
{
    public interface IInfrastructure<T> where T :  class
    {
        Task Add(T model);
        Task Update(T model);
        Task Delete(T model);
        Task<T> Get(int id);
        Task<ICollection<T>> GetAll();
    }
}
