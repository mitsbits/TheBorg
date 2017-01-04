using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;

namespace Borg.Infra.EF6
{
    public abstract class DiscoveryDbContext : DbContext, IDiscoveryDbContext, IHaveAssemblyScanner
    {
        private readonly string _schemaName;

        protected DiscoveryDbContext(DiscoveryDbContextSpec spec) : base(spec.ConnectionStringOrName)
        {
            Providers = spec.GetProviders();
            _schemaName = spec.SchemaName;
        }

        public IEnumerable<IAssemblyProvider> Providers { get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!string.IsNullOrWhiteSpace(_schemaName)) modelBuilder.HasDefaultSchema(_schemaName);

            DiscoverEntities(modelBuilder);
        }

        private void DiscoverEntities(DbModelBuilder modelBuilder)
        {
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
                        var entityInvocation = entityMethod.MakeGenericMethod(type)
                               .Invoke(modelBuilder, new object[] { });
                        if (type.IsSequenceEntity())
                        {
                            modelBuilder.SetKeys(type, new[] { "Id" }, entityInvocation);
                            modelBuilder.SetHasDatabaseGeneratedOption(type, "Id", DatabaseGeneratedOption.None, entityInvocation);
                        }
                    }
                }
            }
        }
    }
}