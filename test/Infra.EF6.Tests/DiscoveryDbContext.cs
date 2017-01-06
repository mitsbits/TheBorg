using Borg.Infra.EF6.Discovery;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using Xunit;

namespace Borg.Infra.EF6.Tests
{
    public class DiscoveryDbContextTests
    {
        private readonly DiscoveryDbContextSpec _spec;
        private readonly TestContext _db;

        public DiscoveryDbContextTests()
        {
            var providers = new List<IAssemblyProvider>(new IAssemblyProvider[] { new CurrentContextAssemblyProvider() });
            _spec = new DiscoveryDbContextSpec() { AssemblyProviders = providers, ConnectionStringOrName = @"data source=.\X2014;initial catalog=RDMS;User Id=dbuser;Password=qazwsx123!@#;App=RDMS Web;" };
            _db = new TestContext(_spec);
        }

        [Fact]
        public void test()
        {
            Should.NotThrow(() =>
            {
                var t = _db.Set<GuidClass>();
                t.Add(new GuidClass() { Name = "xgxdf", Id = Guid.NewGuid() });
            });
        }
    }

    public class TestContext : DiscoveryDbContext
    {
        public TestContext(DiscoveryDbContextSpec spec) : base(spec)
        {
        }
    }

    [MapEntity]
    public class MySequecedClass : SequenceEntity
    {
        public string Name { get; set; }
    }

    [MapEntity]
    public class RogueClass
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public byte[] Data { get; set; }
    }

    [MapEntity]
    public class GuidClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    [MapEntity]
    public class WithConfigurationClass
    {
        public int RealId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public class WithConfigurationClassConfig : EntityTypeConfiguration<WithConfigurationClass>
        {
            public WithConfigurationClassConfig()
            {
                HasKey(x => x.RealId);
            }
        }
    }
}