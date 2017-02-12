using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Borg.Client.Areas.Backoffice.Data;

namespace client.Areas.Backoffice.Data.Migrations.SystemDb
{
    [DbContext(typeof(SystemDbContext))]
    [Migration("20170212210749_InitialSystemDbMigration")]
    partial class InitialSystemDbMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Framework.System.Domain.Page", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivationStatus");

                    b.Property<string>("Body");

                    b.Property<string>("CQRSKey");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Pages");
                });
        }
    }
}
