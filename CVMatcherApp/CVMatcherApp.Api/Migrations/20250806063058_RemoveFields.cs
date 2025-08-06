using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatcherApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMatches_Results_ResultId",
                table: "JobMatches");

            migrationBuilder.AlterColumn<int>(
                name: "ResultId",
                table: "JobMatches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_JobMatches_Results_ResultId",
                table: "JobMatches",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMatches_Results_ResultId",
                table: "JobMatches");

            migrationBuilder.AlterColumn<int>(
                name: "ResultId",
                table: "JobMatches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobMatches_Results_ResultId",
                table: "JobMatches",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
