using Borg.Framework.GateKeeping;
using Borg.Framework.GateKeeping.Data;
using Borg.Framework.GateKeeping.Models;
using Borg.Framework.GateKeeping.Queries;
using Borg.Framework.GateKeeping.Queries.Borg.Framework.Backoffice.Identity.Queries;
using Borg.Framework.GateKeeping.Services;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Borg.Framework.GateKeeping.Commands;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBorgGateKeeping<TSettings>(this IServiceCollection services, TSettings settings) where TSettings : BorgSettings
        {
            services.AddDbContext<GateKeepingDbContext>(options =>
                options.UseSqlServer(settings.Backoffice.Application.Data.Relational.ConnectionStringIndex["identity"]));

            services.AddIdentity<BorgUser, IdentityRole>()
                .AddEntityFrameworkStores<Borg.Framework.GateKeeping.Data.GateKeepingDbContext>()
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
                .AddInMemoryIdentityResources(Borg.Framework.GateKeeping.Resources.GetIdentityResources())
                .AddInMemoryApiResources(Borg.Framework.GateKeeping.Resources.GetApiResources())
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

            services.AddScoped<IHandlesQueryRequest<UsersQueryRequest>, UsersQueryRequestHandler>();

            services.AddScoped<IRepository, IdentityDbTransactionRepository<BorgUser>>();
            services.AddScoped<ICRUDRespoditory<BorgUser>, IdentityDbTransactionRepository<BorgUser>>();
            services.AddScoped<IRepository, IdentityDbQueryRepository<BorgUser>>();
            services.AddScoped<IQueryRepository<BorgUser>, IdentityDbQueryRepository<BorgUser>>();
            services.AddSingleton<IDbContextFactory<GateKeepingDbContext>, IdentityDbContextFactory>();


            services.AddScoped<IHandlesCommand<UserAvatarCommand>, UserAvatarCommandHandler>();
        }
    }
}