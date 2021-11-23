using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class NodeChanges2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Snapshot_Nodes_NodeId",
                table: "Snapshot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Snapshot",
                table: "Snapshot");

            migrationBuilder.RenameTable(
                name: "Snapshot",
                newName: "NodeSnapshots");

            migrationBuilder.RenameIndex(
                name: "IX_Snapshot_NodeId",
                table: "NodeSnapshots",
                newName: "IX_NodeSnapshots_NodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NodeSnapshots",
                table: "NodeSnapshots",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeSnapshots_Nodes_NodeId",
                table: "NodeSnapshots",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeSnapshots_Nodes_NodeId",
                table: "NodeSnapshots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NodeSnapshots",
                table: "NodeSnapshots");

            migrationBuilder.RenameTable(
                name: "NodeSnapshots",
                newName: "Snapshot");

            migrationBuilder.RenameIndex(
                name: "IX_NodeSnapshots_NodeId",
                table: "Snapshot",
                newName: "IX_Snapshot_NodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Snapshot",
                table: "Snapshot",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshot_Nodes_NodeId",
                table: "Snapshot",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
