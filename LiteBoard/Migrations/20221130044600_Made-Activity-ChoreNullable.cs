using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteBoard.Migrations
{
    /// <inheritdoc />
    public partial class MadeActivityChoreNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Chore_ChoreId",
                table: "Activity");

            migrationBuilder.AlterColumn<int>(
                name: "ChoreId",
                table: "Activity",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Chore_ChoreId",
                table: "Activity",
                column: "ChoreId",
                principalTable: "Chore",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Chore_ChoreId",
                table: "Activity");

            migrationBuilder.AlterColumn<int>(
                name: "ChoreId",
                table: "Activity",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Chore_ChoreId",
                table: "Activity",
                column: "ChoreId",
                principalTable: "Chore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
