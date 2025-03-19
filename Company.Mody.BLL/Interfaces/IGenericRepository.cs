using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.DAL.Models;

namespace Company.Mody.BLL.Interfaces
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();
        T? Get(int id);
        int Add(T entity);
        int Update(T entity);
        int Delete(T entity);
        List<T> GetByName(string name);

    }
}
