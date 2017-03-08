using Borg.Framework.Backoffice.Pages.Commands;
using Borg.Framework.Backoffice.Pages.Data;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.MVC.BuildingBlocks.Interactions;
using Borg.Framework.System;
using Borg.Infra;
using Borg.Infra.CQRS;
using Borg.Infra.EFCore;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using Borg.Framework.GateKeeping.Data.Seeds;
using Borg.Framework.UserNotifications;

namespace Borg.Framework.Backoffice
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Environment = env;
            var apppath = env.ContentRootPath;
            var srcPath = apppath.Substring(0, apppath.LastIndexOf('\\'));
            var builder = new ConfigurationBuilder()
                .SetBasePath(srcPath)
                .AddJsonFile("global.appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration.GetSection("global:backoffice")).CreateLogger();
        }

        //TODO: move to configuration

        public IHostingEnvironment Environment { get; }
        public IConfigurationRoot Configuration { get; }

        private BorgSettings Settings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Settings = new BorgSettings();
            services.ConfigureOptions(Configuration.GetSection("global"), () => Settings);

            var scanner = new CurrentContextAssemblyProvider();

            services.AddDistributedMemoryCache();

            services.AddSession(options => options.CookieSecure = CookieSecurePolicy.SameAsRequest);

            services.AddDbContext<PagesDbContext>(options =>
                options.UseSqlServer(Settings.Backoffice.Application.Data.Relational.ConnectionStringIndex["borg"]));

            services.AddScoped<IPageContentAccessor<IPageContent>, DefaultDeviceAccessor>();
            services.AddScoped<IDeviceAccessor<IDevice>, DefaultDeviceAccessor>();

            services.AddSingleton<ISystemService<BorgSettings>, SystemService>();
            services.AddSingleton<IBackofficeService<BorgSettings>, BackofficeService>();
            services.AddSingleton<IBroadcaster, DefaultBroadcaster>();
            services.AddSingleton<IMessagePublisher, InMemoryMessageBus>();

            services.AddSingleton<IDbContextFactory, FactoryDbContextFactory>();
            services.AddSingleton<IDbContextScopeFactory, ServiceLocatorDbContextScopeFactory>();
            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();

            services.AddScoped<IRepository, PagesDbRepository<SimplePage>>();
            services.AddScoped<ICRUDRespoditory<SimplePage>, PagesDbRepository<SimplePage>>();
            services.AddScoped<IHandlesCommand<SimplePageCreateCommand>, SimplePageCreateCommandHandler>();

            services.AddSingleton<IDispatcherInstance, ServiceLocatorDispatcher>();
            services.AddSingleton<ICommandBus, ServiceLocatorDispatcher>();
            services.AddSingleton<IEventBus, ServiceLocatorDispatcher>();
            services.AddSingleton<IQueryBus, ServiceLocatorDispatcher>();

            services.AddSingleton(new DbContextFactoryOptions()
            {
                ContentRootPath = Environment.ContentRootPath,
                ApplicationBasePath = Environment.WebRootPath,
                EnvironmentName = Environment.EnvironmentName
            });

            services.AddSingleton<IDbContextFactory<PagesDbContext>, PagesDbContextFactory>();

            services.AddScoped<IServerResponseProvider, TempDataResponseProvider>();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.CookieHttpOnly = true;
            });
            services.AddBorgUserNotifications(Settings);
            services.AddBorgUserNotificationsForSql(Settings);
            services.AddBorgMedia(Settings);
            services.AddBorgGateKeeping(Settings);
            services.AddBorgHost(Settings);

            services.AddScoped<ISerializer, JsonNetSerializer>();

            services.AddMvc()/*.AddRazorOptions(options => options.AddEmbeddedAdminLteViewsForBackOffice())*/;
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            IdentityDbSeed.InitialiseIdentity(app);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();
            //app.UseEmbeddedAdminLteStaticFilesForBackOffice();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"Areas")),
                RequestPath = new PathString("/Areas")
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Settings.Backoffice.Application.Storage.SharedFolder, Settings.Backoffice.Application.Storage.MediaFolder)),
                RequestPath = new PathString($"/{Settings.Backoffice.Application.Storage.MediaFolder}")
            });

            app.UseIdentity();
            //app.UseIdentityServer();
            app.UseSession(new SessionOptions() { CookieSecure = CookieSecurePolicy.SameAsRequest });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                //routes.MapRoute(
                //    name: "defaultRoute",
                //    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}