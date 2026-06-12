using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicNew.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeAndContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Stories",
                type: "text",
                nullable: false,
                defaultValue: "Comic");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Chapters",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Chapters");
        }
    }
}
