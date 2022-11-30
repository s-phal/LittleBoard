using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteBoard.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityChores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChoreId",
                table: "Activity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ChoreId",
                table: "Activity",
                column: "ChoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Chore_ChoreId",
                table: "Activity",
                column: "ChoreId",
                principalTable: "Chore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Chore_ChoreId",
                table: "Activity");

            migrationBuilder.DropIndex(
                name: "IX_Activity_ChoreId",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "ChoreId",
                table: "Activity");
        }
    }
}
