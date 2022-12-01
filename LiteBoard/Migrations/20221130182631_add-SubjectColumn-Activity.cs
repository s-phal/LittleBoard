using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteBoard.Migrations
{
    /// <inheritdoc />
    public partial class addSubjectColumnActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Activity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Activity");
        }
    }
}
