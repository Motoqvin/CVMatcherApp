using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatcherApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchScore",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "Suggestions",
                table: "CVs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchScore",
                table: "CVs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Suggestions",
                table: "CVs",
                type: "text",
                nullable: true);
        }
    }
}
