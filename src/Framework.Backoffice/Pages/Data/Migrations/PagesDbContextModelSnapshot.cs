using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Borg.Framework.Backoffice.Pages.Data;

namespace Framework.Backoffice.Migrations
{
    [DbContext(typeof(PagesDbContext))]
    partial class PagesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Borg.Framework.Backoffice.Pages.Data.SimplePage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivationStatus");

                    b.Property<string>("Body");

                    b.Property<string>("CQRSKey");

                    b.Property<string>("Path")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.ToTable("SimplePages");
                });
        }
    }
}
