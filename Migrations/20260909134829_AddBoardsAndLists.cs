using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardsAndLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Board_Usuarios_ownerId",
                table: "Board");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardList_Board_BoardId",
                table: "BoardList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Board",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "OwenerId",
                table: "Board");

            migrationBuilder.RenameTable(
                name: "Board",
                newName: "Boards");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                table: "Boards",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Board_ownerId",
                table: "Boards",
                newName: "IX_Boards_OwnerId");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Boards",
                table: "Boards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardList_Boards_BoardId",
                table: "BoardList",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Usuarios_OwnerId",
                table: "Boards",
                column: "OwnerId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardList_Boards_BoardId",
                table: "BoardList");

            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Usuarios_OwnerId",
                table: "Boards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Boards",
                table: "Boards");

            migrationBuilder.RenameTable(
                name: "Boards",
                newName: "Board");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Board",
                newName: "ownerId");

            migrationBuilder.RenameIndex(
                name: "IX_Boards_OwnerId",
                table: "Board",
                newName: "IX_Board_ownerId");

            migrationBuilder.AlterColumn<int>(
                name: "ownerId",
                table: "Board",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "OwenerId",
                table: "Board",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Board",
                table: "Board",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Board_Usuarios_ownerId",
                table: "Board",
                column: "ownerId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardList_Board_BoardId",
                table: "BoardList",
                column: "BoardId",
                principalTable: "Board",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
