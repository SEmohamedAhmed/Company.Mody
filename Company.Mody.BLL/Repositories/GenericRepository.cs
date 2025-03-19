using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.BLL.Interfaces;
using Company.Mody.DAL.Data.Contexts;
using Company.Mody.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Mody.BLL.Repositories
{
    public class GenericRepository<T>:IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context; // Null

        // Ask CLR to create object from AppDbContext
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public T? Get(int id)
        {
            //return _context.Set<T>().FirstOrDefault(d => d.Id == id);

            if (typeof(T) == typeof(Employee))
                return _context.Employees.Include(e=>e.Department).FirstOrDefault(x => x.Id == id) as T;


            return _context.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            if(typeof(T) == typeof(Employee))
                return (IEnumerable < T >) _context.Set<Employee>().Include(e=>e.Department).ToList();

            return _context.Set<T>().ToList();
        }
        public int Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return _context.SaveChanges();
        }
        public int Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return _context.SaveChanges();
        }

        public int Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return _context.SaveChanges();
        }

        public List<T> GetByName(string name)
        {
            if(typeof (T) == typeof(Employee))
                return _context.Employees.Include(e => e.Department).Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList() as List<T>;
            else if(typeof (T) == typeof(Department))
                return _context.Departments.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList() as List<T>;

            throw new NotImplementedException();
        }

    }
}
