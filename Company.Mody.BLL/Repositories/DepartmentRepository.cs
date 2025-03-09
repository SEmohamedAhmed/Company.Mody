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
    // CLR
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context; // Null

        // Ask CLR to create object from AppDbContext
        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Department Get(int? id)
        {
            //return _context.Departments.FirstOrDefault(d => d.Id == id);
            return _context.Departments.Find(id);
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments.ToList();
        }
        public int Add(Department entity)
        {
            _context.Departments.Add(entity);
            return _context.SaveChanges();
        }
        public int Update(Department entity)
        {
            _context.Departments.Update(entity);
            return _context.SaveChanges();
        }

        public int Delete(Department entity)
        {
            _context.Departments.Remove(entity);
            return _context.SaveChanges();
        }




    }
}
