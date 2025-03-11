using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.BLL.Interfaces;
using Company.Mody.DAL.Data.Contexts;
using Company.Mody.DAL.Models;

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

        public T Get(int id)
        {
            //return _context.Set<T>().FirstOrDefault(d => d.Id == id);
            return _context.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
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
    }
}
