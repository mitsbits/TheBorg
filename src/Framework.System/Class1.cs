using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Borg.Framework.System
{



    public class BorgSettings
    {
        public SystemSettings System { get; set; }
        public BackofficeSettings Backoffice { get; set; }
    }

    public class SystemSettings : IBorgSystemSettings
    {
        public string Framework { get; set; }
        public string Version { get; set; }
    }

    public class BackofficeSettings: IBorgSystemSettings
    {
        public string Framework { get; set; }
        public string Version { get; set; }
        public ApplicationSettings Application { get; set; }
        public PagerSettings Pager { get; set; } = new PagerSettings();
    }
    public class PagerSettings
    {
        public int DefaultRowCount { get; set; } = 10;
        public int MaxRowCount { get; set; } = 500;
        public string PageVariable { get; set; } = "p";
        public string RowsVariable { get; set; } = "r";
    }

    public class ApplicationSettings
    {
        public string Title { get; set; }
        public string Logo { get; set; }
        public DataSettings Data { get; set; }
    }

    public class DataSettings
    {
        public RelationalSettings Relational { get; set; }
    }

    public class RelationalSettings
    {
        public ConnectionStringSettings[] ConnectionStrings { get; set; }

        public IReadOnlyDictionary<string, string> ConsectionStringIndex => ConnectionStrings.ToDictionary(x => x.Key, x => x.Value);
    }

    public class ConnectionStringSettings
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public interface IBorgSystemSettings
    {
        string Framework { get; }
        string Version { get; }
    }



}


namespace Microsoft.Extensions.Configuration
{
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