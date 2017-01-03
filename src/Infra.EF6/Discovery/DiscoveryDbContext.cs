using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Reflection;

namespace Borg.Infra.EF6
{
    public interface IDiscoveryDbContext
    {
    }

    public abstract class DiscoveryDbContext : DbContext, IHaveAssemblyScanner
    {
        protected DiscoveryDbContext(DiscoveryDbContextSpec spec) : base(spec.ConnectionStringOrName)
        {
            Providers = spec.GetProviders();
        }

        public IEnumerable<IAssemblyProvider> Providers { get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            var addMethod = typeof(ConfigurationRegistrar)
               .GetMethods()
               .Single(m => m.Name == "Add" && m.GetGenericArguments().Any(a => a.Name == "TEntityType"));

            var asmbls = Providers
                .SelectMany(x => x.Assemblies())
                .Distinct();

            foreach (var assembly in asmbls)
            {
                var entityTypes = assembly
                    .GetTypes()
                    .Where(t => t.IsMapEntity());

                foreach (var type in entityTypes)
                {
                    var configType = typeof(EntityTypeConfiguration<>).MakeGenericType(type);

                    if (assembly.GetTypes().Any(t => t == configType))
                    {
                        var entityConfig = assembly.CreateInstance(configType.FullName);
                        addMethod.MakeGenericMethod(type)
                            .Invoke(modelBuilder.Configurations, new[] { entityConfig });
                    }
                    else
                    {
                        entityMethod.MakeGenericMethod(type)
                            .Invoke(modelBuilder, new object[] { });
                    }
                }
            }
        }
    }

    public class DiscoveryDbContextSpec
    {
        public string SchemaName { get; set; } = "dbo";
        public string ConnectionStringOrName { get; set; } = "default";
        public IEnumerable<IAssemblyProvider> AssemblyProviders { get; set; }

        public IEnumerable<IAssemblyProvider> GetProviders(bool createDefaultIfEmpty = true, Func<Assembly, bool> createDefaultIfEmptyFilter = null)
        {
            if (AssemblyProviders != null && AssemblyProviders.Any()) return AssemblyProviders;
            return (createDefaultIfEmpty) ? new[] { new CurrentContextAssemblyProvider(createDefaultIfEmptyFilter) } : new IAssemblyProvider[0];
        }
    }
}