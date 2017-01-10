using Borg.Infra.CQRS;
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
            _spec = new DiscoveryDbContextSpec() { AssemblyProviders = providers, ConnectionStringOrName = @"data source=.\X2014;initial catalog=testdbcontext;User Id=sa;Password=k0l0k1th0p1t@;App=test;" };
            _db = new TestContext(_spec);
        }

        [Fact]
        public void test()
        {
            Should.NotThrow(() =>
           {
               var t = _db.Set<WithConfigurationClass>();
               var tt = _db.Set<MySequecedClass>();
               t.Add(new WithConfigurationClass() { Name = "jdrtnngnhhn" });
               tt.Add(new MySequecedClass() { Name = "sdfaszvzs" });
               _db.SaveChanges();
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
    [MapSequenceEntity("Id")]
    public class MySequecedClass : Entity<int>
    {
        public string Name { get; set; }
    }

    [MapEntity]
    public class RogueClass : Entity<Guid>
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