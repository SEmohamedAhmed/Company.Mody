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
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context; // Null


        // Ask CLR to create object from AppDbContext
        public EmployeeRepository(AppDbContext context): base(context)
        {

        }

        




    }
}
