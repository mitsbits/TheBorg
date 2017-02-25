using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Borg.Framework.Backoffice.Assets.Data.Migrations.AssetsDb
{
    public partial class looseextraweight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentVersionId",
                table: "Assets");

            migrationBuilder.CreateTable(
                name: "AssetSequence",
                columns: table => new
                {
                    NextId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSequence", x => x.NextId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSequence");

            migrationBuilder.AddColumn<int>(
                name: "CurrentVersionId",
                table: "Assets",
                nullable: false,
                defaultValue: 0);
        }
    }
}
