using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Borg.Framework.Backoffice.Data.Migrations.UserNotificationsDb
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationIdentifier = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Acknowledged = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    RecipientIdentifier = table.Column<string>(maxLength: 256, nullable: true),
                    ResponseStatus = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Title = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationIdentifier);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
