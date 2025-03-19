using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Mody.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Company.Mody.DAL.Data.Configurations
{
    internal class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(e => e.Salary).
                HasColumnType("decimal(18,2)");

            builder.HasOne(e=>e.Department)
                .WithMany(d=>d.Employees)
                .HasForeignKey(e=>e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
