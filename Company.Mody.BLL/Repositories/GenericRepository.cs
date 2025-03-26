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

        public async Task<T?> GetAsync(int id)
        {
            //return _context.Set<T>().FirstOrDefault(d => d.Id == id);

            if (typeof(T) == typeof(Employee))
                return await _context.Employees.Include(e=>e.Department).FirstOrDefaultAsync(x => x.Id == id) as T;


            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if(typeof(T) == typeof(Employee))
                return (IEnumerable<T>) await _context.Set<Employee>().Include(e=>e.Department).ToListAsync();

            return await _context.Set<T>().ToListAsync();
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<List<T>> GetByNameAsync(string name)
        {
            if(typeof (T) == typeof(Employee))
                return await _context.Employees.Include(e => e.Department).Where(e => e.Name.ToLower().Contains(name.ToLower())).ToListAsync() as List<T>;
            else if(typeof (T) == typeof(Department))
                return await _context.Departments.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToListAsync() as List<T>;

            throw new NotImplementedException();
        }


    }
}
