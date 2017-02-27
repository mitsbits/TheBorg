using System;
using Borg.Framework.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Borg.Framework.Backoffice.Data.Migrations.AssetsDb
{
    [DbContext(typeof(AssetsDbContext))]
    partial class AssetsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:Sequence:.AssetsSequence", "'AssetsSequence', '', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Borg.Framework.Media.Data.AssetSpec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("NEXT VALUE FOR AssetsSequence");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 512);

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("Borg.Framework.Media.Data.FileSpec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("FullPath");

                    b.Property<DateTime?>("LastRead");

                    b.Property<DateTime>("LastWrite");

                    b.Property<string>("MimeType");

                    b.Property<string>("Name");

                    b.Property<long>("SizeInBytes");

                    b.Property<int>("VersionId");

                    b.HasKey("Id");

                    b.HasIndex("VersionId")
                        .IsUnique();

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Borg.Framework.Media.Data.VersionSpec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssetId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.ToTable("Versions");
                });

            modelBuilder.Entity("Borg.Framework.Media.Data.FileSpec", b =>
                {
                    b.HasOne("Borg.Framework.Media.Data.VersionSpec", "Version")
                        .WithOne("FileSpec")
                        .HasForeignKey("Borg.Framework.Media.Data.FileSpec", "VersionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Borg.Framework.Media.Data.VersionSpec", b =>
                {
                    b.HasOne("Borg.Framework.Media.Data.AssetSpec", "Asset")
                        .WithMany("Versions")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
