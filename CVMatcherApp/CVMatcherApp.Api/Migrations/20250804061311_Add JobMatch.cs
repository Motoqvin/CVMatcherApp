using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CVMatcherApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddJobMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchScore",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Suggestions",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Results");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Results",
                newName: "ResultId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Results",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Results",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Results",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ResultId",
                table: "CVs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JobMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnalysisResultId = table.Column<int>(type: "integer", nullable: false),
                    JobDescription = table.Column<string>(type: "text", nullable: false),
                    MatchScore = table.Column<int>(type: "integer", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobMatches_Results_AnalysisResultId",
                        column: x => x.AnalysisResultId,
                        principalTable: "Results",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CVs_ResultId",
                table: "CVs",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_JobMatches_AnalysisResultId",
                table: "JobMatches",
                column: "AnalysisResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Results_ResultId",
                table: "CVs",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "ResultId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Results_ResultId",
                table: "CVs");

            migrationBuilder.DropTable(
                name: "JobMatches");

            migrationBuilder.DropIndex(
                name: "IX_CVs_ResultId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "ResultId",
                table: "CVs");

            migrationBuilder.RenameColumn(
                name: "ResultId",
                table: "Results",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "MatchScore",
                table: "Results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Suggestions",
                table: "Results",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Results",
                type: "text",
                nullable: true);
        }
    }
}
