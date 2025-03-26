using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.BLL.Interfaces;
using Company.Mody.DAL.Data.Contexts;

namespace Company.Mody.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IDepartmentRepository DepartmentRepository { get; }

        public IEmployeeRepository EmployeeRepository { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            DepartmentRepository = new DepartmentRepository(context);
            EmployeeRepository = new EmployeeRepository(context);
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
