using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddNodeDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastBackupDateUtc",
                table: "Nodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCleanedDateUtc",
                table: "Nodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSnapshotDateUtc",
                table: "Nodes",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastBackupDateUtc",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LastCleanedDateUtc",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LastSnapshotDateUtc",
                table: "Nodes");
        }
    }
}
