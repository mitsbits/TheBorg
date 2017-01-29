using Autofac;
using Autofac.Extensions.DependencyInjection;
using Borg.Client.Models;
using Borg.Framework.Redis;
using Borg.Framework.Redis.Messaging;
using Borg.Infra;
using Borg.Infra.Caching;
using Borg.Infra.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;
using Borg.Client.Controllers;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks;

namespace Borg.Client
{
    public class Startup
    {
        public static IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDistributedCache>(
              serviceProvider =>
                  new RedisCache(new RedisCacheOptions
                  {
                      Configuration = Configuration["Redis:Url"],
                      InstanceName = Configuration["Redis:Name"]
                  }));

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.CookieHttpOnly = true;
            });

            services.AddMvc().AddControllersAsServices(); 

            var builder = new ContainerBuilder();

            builder.RegisterType<JsonNetSerializer>().As<ISerializer>().WithParameter("settings", new JsonSerializerSettings() { Formatting = Formatting.Indented }).InstancePerDependency();

            var multiplexer = ConnectionMultiplexer.Connect(Configuration["Redis:Url"]);
            var subscriber = multiplexer.GetSubscriber();
            builder.Register((c, p) => new RedisMessageBus(subscriber, CacheConstants.CACHE_DEPEDENCY_TOPIC, c.Resolve<ISerializer>()))
                .Named<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC).As<IMessagePublisher>()
                .SingleInstance();
            builder.Register((c, p) => new RedisDepedencyCacheClient(multiplexer, c.ResolveNamed<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC), c.Resolve<ISerializer>()))
                .As<IDepedencyCacheClient>()
                .SingleInstance();

            builder.RegisterType<AppBroadcaster>().As<IBroadcaster>().SingleInstance();
            builder.RegisterType<DefaultDeviceAccessor>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Populate(services);

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(FrameworkController)))
                .PropertiesAutowired();
               


            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        }
    }
}