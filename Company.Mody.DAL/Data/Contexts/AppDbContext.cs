using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Company.Mody.DAL.Data.Contexts
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        //public DbSet<IdentityUser> Users { get; set; }
        //public DbSet<IdentityRole> Roles { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }



        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server = .; database = MVC_Mody_Company; trusted_connection=true; TrustServerCertificate = true;");
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // important if u r inherting from a class that does configs e.g IdentityDbContext class
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // to ignore some inherited props use below syntax
            //modelBuilder.Entity<AppUser>()
            //    .Ignore(u => u.ConcurrencyStamp);

        
        }




    }

}
