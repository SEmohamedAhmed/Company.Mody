using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Company.Mody.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(); // for MVC apps

            //builder.Services.AddScoped<AppDbContext>(); // allow DI for appdbcontext

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });


            //builder.Services.AddScoped<DepartmentRepository>(); // allows DI for DepartmentRepository
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // allows DI for DepartmentRepository
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // allows DI for DepartmentRepository

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
