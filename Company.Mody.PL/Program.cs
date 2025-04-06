using System.Security.Claims;
using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Data.Contexts;
using Company.Mody.DAL.Models;
using Company.Mody.PL.Mapping;
using Company.Mody.PL.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Company.Mody.PL.Helper.TwilioSms;
using Company.Mody.PL.Helper.MailKitHelper;
using Company.Mody.PL.Helper.Bitly;

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


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Signin"; // Redirect to Signin on unauthorized access
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Default expiration time
                //options.SlidingExpiration = false; // Prevents automatic extension
                //options.Cookie.HttpOnly = true; // Security best practice
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures secure transmission in HTTPS
            });




            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
            builder.Services.AddScoped<IMailService, MailService>();


            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection(nameof(TwilioSettings)));
            builder.Services.AddScoped<ITwilioService, TwilioService>();


            builder.Services.Configure<BitlySettings>(builder.Configuration.GetSection(nameof(BitlySettings)));
            builder.Services.AddScoped<IBitlyService, BitlyService>();


            // This prevents users who are deleted and has remeber me cookies from using the website
            // Deletes .AspNetCore.Identity.Application from user cookies
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(5); // Check every 5 seconds
                //options.ValidationInterval = TimeSpan.FromMinutes(1); // Check every 1 Minute
            });



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



            // Add Google authentication
            builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google"; // Default
            })
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
            });




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
