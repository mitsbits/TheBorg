using Borg.Framework.Media;
using Borg.Framework.Media.EventHandlers;
using Borg.Framework.Media.Services;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using Borg.Infra.Storage;
using Borg.Infra.Storage.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBorgMedia<TSettings>(this IServiceCollection services, TSettings settings) where TSettings : BorgSettings
        {
            services.AddSingleton<IBorgPlugin, MediaPlugin>();

            services.AddDbContext<AssetsDbContext>(options =>
               options.UseSqlServer(settings.Backoffice.Application.Data.Relational.ConnectionStringIndex["borg"]));

            services.AddScoped<IRepository, AssetsDbRepository<AssetSpec>>();

            services.AddScoped<ICRUDRespoditory<AssetSpec>, AssetsDbRepository<AssetSpec>>();

            services.AddSingleton<IDbContextFactory<AssetsDbContext>, AssetsDbContextFactory>();

            services.AddScoped<IAssetMetadataStorage<int>, AssetsMetadataStorage>();

            services.AddTransient<AssetSequence>();

            services.AddScoped<IMediaService, MediaService>(provider =>
            {
                var storage = new FolderFileStorage(Path.Combine(settings.Backoffice.Application.Storage.SharedFolder, settings.Backoffice.Application.Storage.MediaFolder));

                return new MediaService(provider.GetService<ILoggerFactory>(), storage, provider.GetService<AssetSequence>(),
                    new DefaultConflictingNamesResolver(), provider.GetService<IAssetMetadataStorage<int>>(), new DefaultFolderIntegerScopeFactory(), provider.GetService<AssetsDbContext>(), provider.GetService<IEventBus>());
            });

            services.AddSingleton<IAssetUrlResolver, AssetUrlResolver>();

            services.AddScoped<IHandlesEvent<FileAddedToAssetEvent<int>>, CacheNewImage>();
        }
    }
}