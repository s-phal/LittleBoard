using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteBoard.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Project",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Project");
        }
    }
}
