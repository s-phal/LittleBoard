using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteBoard.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedDateChores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Chores",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Chores");
        }
    }
}
