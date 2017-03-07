using Borg.Infra.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.UserNotifications.Sql
{
    public class UserNotificationsDbContext : DbContext
    {
        internal UserNotificationsDbContext() : base()
        {
        }

        public UserNotificationsDbContext(DbContextOptions<UserNotificationsDbContext> options)
            : base(options)
        {
        }

        internal DbSet<UserNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserNotification>()
                .Property(x => x.NotificationIdentifier).HasColumnType($"char(36)");
        }
    }

    internal class UserNotification : IUserNotification
    {
        protected internal UserNotification()
        {
        }

        [Key]
        [MaxLength(36)]
        public string NotificationIdentifier { get; protected set; } = Guid.NewGuid().ToString();

        [MaxLength(256)]
        public string RecipientIdentifier { get; set; }

        [MaxLength(1024)]
        public string Title { get; set; }

        public string Message { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
        public DateTimeOffset Timestamp { get; protected set; } = DateTimeOffset.UtcNow;
        public bool Acknowledged { get; set; } = false;
    }
}