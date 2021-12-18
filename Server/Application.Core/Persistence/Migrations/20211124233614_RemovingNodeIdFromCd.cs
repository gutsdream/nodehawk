using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class RemovingNodeIdFromCd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NodeId",
                table: "ConnectionDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUtc",
                table: "NodeSnapshots",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateUtc",
                table: "NodeSnapshots");

            migrationBuilder.AddColumn<Guid>(
                name: "NodeId",
                table: "ConnectionDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
