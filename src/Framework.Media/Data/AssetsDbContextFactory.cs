using Borg.Framework.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Framework.Media
{ //TODO:this has to go to another layer
    public class AssetsDbContextFactory : IDbContextFactory<AssetsDbContext>
    {
        private readonly BorgSettings _settings;
        private readonly string _cs = string.Empty;

        public AssetsDbContextFactory(BorgSettings settings)
        {
            _settings = settings;
        }

        public AssetsDbContextFactory()
        {
            //_cs = "Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
            _cs = "Server=.\\SQL2016;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
        }

        public AssetsDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<AssetsDbContext>();
            if (_settings != null)
            {
                var ops =
                    builder.UseSqlServer(_settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["borg"])
                        .Options;
                var context = new AssetsDbContext(ops);
                return context;
            }
            else
            {
                var ops =
                    builder.UseSqlServer(_cs, optionsBuilder =>  optionsBuilder.MigrationsAssembly("Framework.Backoffice"))
                        .Options;
                var context = new AssetsDbContext(ops);
                return context;
            }
        }
    }
}