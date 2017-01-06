using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Borg.Infra.EF6.Discovery;

namespace Borg.Infra.EF6
{
    public abstract class DiscoveryDbContext : DbContext, IDiscoveryDbContext, IHaveAssemblyScanner
    {
        private readonly string _schemaName;
        private readonly bool _tryToCreateSequence = false;

        protected DiscoveryDbContext(DiscoveryDbContextSpec spec) : base(spec.ConnectionStringOrName)
        {
            Providers = spec.GetProviders();
            _schemaName = spec.SchemaName;
            _tryToCreateSequence = spec.TryToCreateSequence;
        }

        public IEnumerable<IAssemblyProvider> Providers { get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!string.IsNullOrWhiteSpace(_schemaName)) modelBuilder.HasDefaultSchema(_schemaName);

            DiscoverEntities(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetNewIdsToTransientSequenceEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(default(CancellationToken));
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SetNewIdsToTransientSequenceEntities();
            return base.SaveChangesAsync(cancellationToken);
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
                            var keyField =  ((MapSequenceEntityAttribute) type.GetCustomAttribute(typeof(MapSequenceEntityAttribute), false)).IdField;
                            modelBuilder.SetKeys(type, new[] { keyField }, entityInvocation);
                            modelBuilder.SetHasDatabaseGeneratedOption(type, "Id", DatabaseGeneratedOption.None, entityInvocation);
                        }
                    }
                }
            }
        }

        private void SetNewIdsToTransientSequenceEntities()
        {
            foreach (var entry in from entry in ChangeTracker.
                      Entries().Where(e => e.State == EntityState.Added)
                                  let entity = entry.Entity
                                  where entity != null && entity.GetType().IsSequenceEntity()
                                  select entry)
            {
                NextSequenceForEntity(entry);
            }
        }

        private void NextSequenceForEntity(DbEntityEntry entry)
        {
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            var wKey =
                objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName, DataSpace.CSpace)
                    .BaseEntitySets.First(meta => meta.ElementType.Name == entry.Entity.GetType().Name)
                    .ElementType.KeyMembers.Select(k => k.Name).FirstOrDefault();
            var keyType = entry.Property(wKey).CurrentValue.GetType();
            if (keyType != typeof(int)) throw new ApplicationException($"Entity {entry.Entity.GetType().Name} must have an integer key. {wKey} is {keyType.Name}");
            if ((int)entry.Property(wKey).CurrentValue == default(int))
                entry.Property(wKey).CurrentValue = this.GetNextIdFromSequence(entry, _tryToCreateSequence);
        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}