using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteBoard.Migrations
{
    /// <inheritdoc />
    public partial class ActionCleanUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Project",
                type: "character varying(35)",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Project",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(35)",
                oldMaxLength: 35);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);
        }
    }
}
