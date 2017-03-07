using Borg.Framework.UserNotifications.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Borg.Framework.Backoffice.Data.Migrations.UserNotificationsDb
{
    [DbContext(typeof(UserNotificationsDbContext))]
    partial class UserNotificationsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Borg.Framework.Sql.UserNotifications.UserNotification", b =>
                {
                    b.Property<string>("NotificationIdentifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasAnnotation("MaxLength", 36);

                    b.Property<bool>("Acknowledged");

                    b.Property<string>("Message");

                    b.Property<string>("RecipientIdentifier")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<int>("ResponseStatus");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 1024);

                    b.HasKey("NotificationIdentifier");

                    b.ToTable("Notifications");
                });
        }
    }
}