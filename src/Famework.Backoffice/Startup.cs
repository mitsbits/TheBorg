using System;
using System.IO;
using Borg.Famework.Backoffice.Identity;
using Borg.Famework.Backoffice.Identity.Models;
using Borg.Famework.Backoffice.Identity.Services;
using Framework.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using IdentityDbContext = Borg.Famework.Backoffice.Identity.Data.IdentityDbContext;

namespace Famework.Backoffice
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var apppath = env.ContentRootPath;
            var rootpath = apppath.Substring(0, apppath.LastIndexOf('\\'));
            var builder = new ConfigurationBuilder()
                .SetBasePath(rootpath)
                .AddJsonFile("global.appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration).CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new BorgSettings();
            services.ConfigurePOCO(Configuration.GetSection("global"), () => settings);

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["identity"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/backoffice/account/login";
                options.Cookies.ApplicationCookie.LogoutPath = "/backoffice/account/logoff";

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.AddIdentityServer()
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                //.AddTestUsers(Users.Get())
                //.AddInMemoryPersistedGrants()
                //.AddConfigurationStore(builder =>
                //    builder.UseSqlServer(idsettings.Database.ConnectionString, options =>
                //        options.MigrationsAssembly(migrationsAssembly)))
                //.AddOperationalStore(builder =>
                //    builder.UseSqlServer(idsettings.Database.ConnectionString, options =>
                //        options.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetIdentity<ApplicationUser>()
                .AddTemporarySigningCredential();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Areas")),
                RequestPath = new PathString("/Areas")
            });

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}
