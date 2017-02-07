using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.Identity.Models;
using Borg.Framework.Identity.Services;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Borg.Framework.Identity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration).CreateLogger();
  

        }

        public IConfigurationRoot Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var idsettings = new IdentityServerSettings();
            services.ConfigurePOCO(Configuration.GetSection("identityServer"), () => idsettings);

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(idsettings.Database.ConnectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
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
                //options.Cookies.ApplicationCookie.LoginPath = "/Account/LogIn";
                //options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOff";

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
                        IApplicationLifetime appLifetime)
        {
           // InitializeDatabase(app);
           // Task.WaitAll(InitialiseIdentity(app));

            //loggerFactory.AddConsole(Configuration);
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentity();
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World from Borg Identity!");
            //});
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }



        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }



        private async Task InitialiseIdentity(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var umanager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var rmanager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var adminrole =
                    await rmanager.Roles.FirstOrDefaultAsync(
                        x => x.NormalizedName.Equals("admin", StringComparison.InvariantCultureIgnoreCase));

                if (adminrole == null)
                {
                    adminrole = new IdentityRole("admin");
                    await rmanager.CreateAsync(adminrole);
                    await rmanager.AddClaimAsync(adminrole, new Claim(JwtClaimTypes.NickName, "adminaras"));
                }

                var user = await umanager.FindByNameAsync("mitsbits");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = "mitsbits@gmail.com",
                        UserName = "mitsbits",
                        LockoutEnabled = false
                    };
                    await umanager.CreateAsync(user, "qazwsx123!@#");
                    await umanager.AddClaimAsync(user, new Claim(JwtClaimTypes.NickName, "mitsaras"));


                }
                await umanager.AddToRoleAsync(user, "admin");

            }
        }





    }
    public static class ServiceCollectionExtensions
    {
        public static TConfig ConfigurePOCO<TConfig>(this IServiceCollection services, IConfiguration configuration,
            Func<TConfig> pocoProvider) where TConfig : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (pocoProvider == null) throw new ArgumentNullException(nameof(pocoProvider));

            var config = pocoProvider();
            configuration.Bind(config);
            services.AddSingleton(config);
            return config;
        }

        public static TConfig ConfigurePOCO<TConfig>(this IServiceCollection services, IConfiguration configuration,
            TConfig config) where TConfig : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (config == null) throw new ArgumentNullException(nameof(config));

            configuration.Bind(config);
            services.AddSingleton(config);
            return config;
        }
    }
}