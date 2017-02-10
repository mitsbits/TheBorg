using Autofac;
using Autofac.Extensions.DependencyInjection;

using Borg.Framework.MVC;
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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Autofac.Core;
using Borg.Client.Models;
using Borg.Infra.CQRS;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Serilog;

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

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration).CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var scanner = new CurrentContextAssemblyProvider();


            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration["Redis:Url"];
                options.InstanceName = "output";
            });
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

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("admin", policyAdmin =>
            //    {
            //        policyAdmin.RequireClaim("role", "admin");
            //    });
            //});

            services.AddMvc().AddControllersAsServices();

            var builder = new ContainerBuilder();

            builder.RegisterType<JsonNetSerializer>().As<ISerializer>().WithParameter("settings", new JsonSerializerSettings() { Formatting = Formatting.Indented }).InstancePerDependency();

            var multiplexer = ConnectionMultiplexer.Connect(Configuration["Redis:Url"]);
            var subscriber = multiplexer.GetSubscriber();

            builder.RegisterType<RedisMessageBus>()
                .Named<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC)
                .WithParameters(new[]
                {
                    new NamedParameter("subscriber", subscriber),
                    new NamedParameter("topic", CacheConstants.CACHE_DEPEDENCY_TOPIC),
                })
                .As<IMessagePublisher>()
                .SingleInstance();

            
            

            builder.RegisterType<RedisDepedencyCacheClient>()
                .WithParameters(new Parameter[]
                {
                    new NamedParameter("connectionMultiplexer", multiplexer),
                    new ResolvedParameter((pi, ctx) => pi.Name == "subscriber", (pi, ctx) => ctx.ResolveNamed<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC))

                 })
                .As<IDepedencyCacheClient>()
                .SingleInstance();

            //builder.Register((c, p) => new RedisMessageBus(subscriber, CacheConstants.CACHE_DEPEDENCY_TOPIC, c.Resolve<ISerializer>()))
            //    .Named<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC).As<IMessagePublisher>()
            //    .SingleInstance();
            //builder.Register((c, p) => new RedisDepedencyCacheClient(multiplexer, c.ResolveNamed<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC), c.Resolve<ISerializer>()))
            //    .As<IDepedencyCacheClient>()
            //    .SingleInstance();

            builder.RegisterType<AppBroadcaster>().As<IBroadcaster>().SingleInstance();
            builder.RegisterType<DefaultDeviceAccessor>().AsImplementedInterfaces().InstancePerLifetimeScope();


            builder.RegisterAssemblyTypes(scanner.Assemblies())
                .AsClosedTypesOf(typeof(IHandlesEvent<>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(scanner.Assemblies())
                .AsClosedTypesOf(typeof(IHandlesCommand<>)).AsImplementedInterfaces();
            builder.RegisterType<AutofacDispatcher>().As<IDispatcherInstance>().SingleInstance();
            builder.RegisterType<AutofacDispatcher>().As<ICommandBus>().SingleInstance();
            builder.RegisterType<AutofacDispatcher>().As<IEventBus>().SingleInstance();

            //builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();


            builder.Populate(services);

            builder.RegisterAssemblyTypes(scanner.Assemblies())
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(FrameworkController)))
                .PropertiesAutowired();

            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationScheme = "Cookies"
            //});

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            //{
            //    AuthenticationScheme = "oidc",
            //    SignInScheme = "Cookies",

            //    Authority = "http://localhost:44383",
            //    RequireHttpsMetadata = false,

            //    ClientId = "mvc",
            //    ClientSecret = "secret",

            //    ResponseType = "code id_token",
            //    Scope = { "api1", "offline_access" },

            //    GetClaimsFromUserInfoEndpoint = true,
            //    SaveTokens = true
            //});

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "cookie"
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {

                ClientId = "openIdConnectClient",
                Authority = "https://localhost:44383/",
                SignInScheme = "cookie",
                RequireHttpsMetadata = false,
                //GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                //ResponseType = "code id_token",
                Scope = { "openid", "profile", "email", "role" },
                TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                }

                //ClientId = "openIdConnectClient",
                //Authority = "https://localhost:44383/",
                //SignInScheme = "cookie",
                //RequireHttpsMetadata = false,
                //GetClaimsFromUserInfoEndpoint = true,
                //SaveTokens = true,
                ////ResponseType = "code id_token",
                ////Scope = { "openid", "profile", "email", "role" },
                //TokenValidationParameters = new TokenValidationParameters()
                //{
                //    NameClaimType = ClaimTypes.Name,
                //    RoleClaimType = ClaimTypes.Role,
                //}

                //AuthenticationScheme = "oidc",
                //SignInScheme = "Cookies",

                //Authority = "https://localhost:44383/",
                //RequireHttpsMetadata = false,

                //ClientId = "openIdConnectClient",
                //ClientSecret = "secret",

                //ResponseType = "code id_token",
                //Scope = { "openid", "profile", "email", "api1", "offline_access", "role" },
                //GetClaimsFromUserInfoEndpoint = true,

                //SaveTokens = true,

                //TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                //{
                //    NameClaimType = JwtClaimTypes.Name,
                //    RoleClaimType = JwtClaimTypes.Role,
                //}

            });

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            //app.UseJwtBearerAuthentication(new JwtBearerOptions()
            //{
            //    Authority = "https://localhost:44383",
            //    Audience = "https://localhost:44383/resources",
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,
            //});


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