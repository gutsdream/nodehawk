using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class NodeChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConnectionDetailsId",
                table: "Nodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ConnectionDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NodeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Host = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NodeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpaceUsedPercentage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snapshot_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ConnectionDetailsId",
                table: "Nodes",
                column: "ConnectionDetailsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Snapshot_NodeId",
                table: "Snapshot",
                column: "NodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_ConnectionDetails_ConnectionDetailsId",
                table: "Nodes",
                column: "ConnectionDetailsId",
                principalTable: "ConnectionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_ConnectionDetails_ConnectionDetailsId",
                table: "Nodes");

            migrationBuilder.DropTable(
                name: "ConnectionDetails");

            migrationBuilder.DropTable(
                name: "Snapshot");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ConnectionDetailsId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ConnectionDetailsId",
                table: "Nodes");
        }
    }
}
