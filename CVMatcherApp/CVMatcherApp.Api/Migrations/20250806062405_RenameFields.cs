using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatcherApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMatches_Results_AnalysisResultId",
                table: "JobMatches");

            migrationBuilder.RenameColumn(
                name: "AnalysisResultId",
                table: "JobMatches",
                newName: "ResultId");

            migrationBuilder.RenameIndex(
                name: "IX_JobMatches_AnalysisResultId",
                table: "JobMatches",
                newName: "IX_JobMatches_ResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobMatches_Results_ResultId",
                table: "JobMatches",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMatches_Results_ResultId",
                table: "JobMatches");

            migrationBuilder.RenameColumn(
                name: "ResultId",
                table: "JobMatches",
                newName: "AnalysisResultId");

            migrationBuilder.RenameIndex(
                name: "IX_JobMatches_ResultId",
                table: "JobMatches",
                newName: "IX_JobMatches_AnalysisResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobMatches_Results_AnalysisResultId",
                table: "JobMatches",
                column: "AnalysisResultId",
                principalTable: "Results",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
