using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Mody.DAL.Data.Contexts
{
    public class AppDbContext:DbContext
    {
        public DbSet<Department> Departments { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }



        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server = .; database = MVC_Mody_Company; trusted_connection=true; TrustServerCertificate = true;");
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }




    }

}
