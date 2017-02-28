﻿using Borg.Framework.Backoffice.Identity;
using Borg.Framework.Backoffice.Identity.Data;
using Borg.Framework.Backoffice.Identity.Models;
using Borg.Framework.Backoffice.Identity.Queries;
using Borg.Framework.Backoffice.Identity.Queries.Borg.Framework.Backoffice.Identity.Queries;
using Borg.Framework.Backoffice.Identity.Services;
using Borg.Framework.Backoffice.Pages.Commands;
using Borg.Framework.Backoffice.Pages.Data;
using Borg.Framework.Media;
using Borg.Framework.Media.EventHandlers;
using Borg.Framework.Media.Services;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra;
using Borg.Infra.CQRS;
using Borg.Infra.EFCore;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using Borg.Infra.Storage;
using Borg.Infra.Storage.Assets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using IdentityDbContext = Borg.Framework.Backoffice.Identity.Data.IdentityDbContext;

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
            services.ConfigurePOCO(Configuration.GetSection("global"), () => Settings);

            var scanner = new CurrentContextAssemblyProvider();



            services.AddDistributedMemoryCache();

            services.AddSession(options => options.CookieSecure = CookieSecurePolicy.SameAsRequest);


            services.AddDbContext<PagesDbContext>(options =>
                options.UseSqlServer(Settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["borg"]));

            services.AddDbContext<AssetsDbContext>(options =>
                options.UseSqlServer(Settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["borg"]));

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(Settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["identity"]));

            services.AddIdentity<BorgUser, IdentityRole>()
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
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(15);
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
                .AddAspNetIdentity<BorgUser>()
                .AddTemporarySigningCredential();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddScoped<IPageContentAccessor<IPageContent>, DefaultDeviceAccessor>();
            services.AddScoped<IDeviceAccessor<IDevice>, DefaultDeviceAccessor>();

            services.AddSingleton<ISystemService<BorgSettings>, SystemService>();

            services.AddSingleton<IDbContextFactory, FactoryDbContextFactory>();
            services.AddSingleton<IDbContextScopeFactory, ServiceLocatorDbContextScopeFactory>();
            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();

            services.AddScoped<IRepository, PagesDbRepository<SimplePage>>();
            services.AddScoped<ICRUDRespoditory<SimplePage>, PagesDbRepository<SimplePage>>();
            services.AddScoped<IHandlesCommand<SimplePageCreateCommand>, SimplePageCreateCommandHandler>();

            services.AddScoped<IHandlesQueryRequest<UsersQueryRequest>, UsersQueryRequestHandler>();

            services.AddScoped<IRepository, IdentityDbTransactionRepository<BorgUser>>();
            services.AddScoped<ICRUDRespoditory<BorgUser>, IdentityDbTransactionRepository<BorgUser>>();
            services.AddScoped<IRepository, IdentityDbQueryRepository<BorgUser>>();
            services.AddScoped<IQueryRepository<BorgUser>, IdentityDbQueryRepository<BorgUser>>();
            services.AddScoped<IRepository, AssetsDbRepository<Media.AssetSpec>>();
            services.AddScoped<ICRUDRespoditory<Media.AssetSpec>, AssetsDbRepository<Media.AssetSpec>>();

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
            services.AddSingleton<IDbContextFactory<IdentityDbContext>, IdentityDbContextFactory>();
            services.AddSingleton<IDbContextFactory<AssetsDbContext>, AssetsDbContextFactory>();

            services.AddScoped<IAssetMetadataStorage<int>, AssetsMetadataStorage>();
            services.AddTransient<AssetSequence>();

            services.AddScoped<IMediaService, MediaService>(provider =>
            {
                var storage = new FolderFileStorage(Path.Combine(Settings.Backoffice.Application.Storage.SharedFolder, Settings.Backoffice.Application.Storage.MediaFolder));

                return new MediaService(provider.GetService<ILoggerFactory>(), storage, provider.GetService<AssetSequence>(),
                    new DefaultConflictingNamesResolver(), provider.GetService<IAssetMetadataStorage<int>>(), new DefaultFolderIntegerScopeFactory(), provider.GetService<AssetsDbContext>(), provider.GetService<IEventBus>());
            });

            services.AddSingleton<IAssetUrlResolver, AssetUrlResolver>();

            services.AddScoped<IHandlesEvent<FileAddedToAssetEvent<int>>, CacheNewImage>();



            services.AddScoped<IServerResponseProvider, TempDataResponseProvider>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.CookieHttpOnly = true;
            });


            services.AddScoped<ISerializer, JsonNetSerializer>();

            services.AddMvc();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            //IdentityDbSeed.InitialiseIdentity(app);

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
            app.UseSession(new SessionOptions() {CookieSecure = CookieSecurePolicy.SameAsRequest});
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