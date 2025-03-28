using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Data.Contexts;
using Company.Mody.DAL.Models;
using Company.Mody.PL.Mapping;
using Company.Mody.PL.Services;
using Microsoft.AspNetCore.Identity;
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
            //builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // allows DI for DepartmentRepository
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // allows DI for UnitOfWork
            builder.Services.AddAutoMapper(typeof(EmployeeProfile));

            //builder.Services.AddScoped<UserManager<AppUser>>();

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(
                options => options.LoginPath="/Account/Signin"

                );


            #region Services Life Time


            // best practice : 
            // Repository/Database          => Scoped
            // Cach/Security                => Singleton


            //builder.Services.AddScoped();       // create object of life time one per request
            //builder.Services.AddTransient();    // create object of life time one per operation (mutiple ops in one request will create multiple objects)
            //builder.Services.AddSingleton();    // create object of life time one per application


            // allows dependency injection for our services

            builder.Services.AddScoped<IScopedService,ScopedService>();         // per request
            builder.Services.AddTransient<ITransientService,TransientService>();   // per operation
            builder.Services.AddSingleton<ISingletonService,SingletonService>();   // per application




            #endregion





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

            // must be after routing
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
