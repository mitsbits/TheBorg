using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Borg.Infra.EFCore;
using Microsoft.EntityFrameworkCore;


namespace Borg.Client.Areas.Backoffice.Data
{
    public class SystemDbContext : DiscoveryDbContext
    {
        public SystemDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}
