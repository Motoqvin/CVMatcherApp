using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatcherApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResultId",
                table: "Results",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Results",
                newName: "ResultId");
        }
    }
}
