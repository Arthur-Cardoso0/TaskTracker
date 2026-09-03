using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardList_Boards_BoardId",
                table: "BoardList");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_BoardList_BoardListId",
                table: "Tarefas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardList",
                table: "BoardList");

            migrationBuilder.RenameTable(
                name: "BoardList",
                newName: "BoardLists");

            migrationBuilder.RenameIndex(
                name: "IX_BoardList_BoardId",
                table: "BoardLists",
                newName: "IX_BoardLists_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardLists",
                table: "BoardLists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardLists_Boards_BoardId",
                table: "BoardLists",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_BoardLists_BoardListId",
                table: "Tarefas",
                column: "BoardListId",
                principalTable: "BoardLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardLists_Boards_BoardId",
                table: "BoardLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_BoardLists_BoardListId",
                table: "Tarefas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardLists",
                table: "BoardLists");

            migrationBuilder.RenameTable(
                name: "BoardLists",
                newName: "BoardList");

            migrationBuilder.RenameIndex(
                name: "IX_BoardLists_BoardId",
                table: "BoardList",
                newName: "IX_BoardList_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardList",
                table: "BoardList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardList_Boards_BoardId",
                table: "BoardList",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_BoardList_BoardListId",
                table: "Tarefas",
                column: "BoardListId",
                principalTable: "BoardList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
