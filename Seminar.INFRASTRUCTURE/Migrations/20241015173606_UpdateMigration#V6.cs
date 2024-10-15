using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Disciplines_DisciplineId",
                table: "Reviewers");

            migrationBuilder.RenameColumn(
                name: "DisciplineId",
                table: "Reviewers",
                newName: "FacultyId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviewers_DisciplineId",
                table: "Reviewers",
                newName: "IX_Reviewers_FacultyId");

            migrationBuilder.RenameColumn(
                name: "IsStatus",
                table: "Acceptances",
                newName: "IsSuccessed");

            migrationBuilder.AddColumn<string>(
                name: "Supervisor",
                table: "ResearchTopics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Competitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Competitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Review_Board_Member",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    ReviewCommitteeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_Board_Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Board_Member_Review_Committees_ReviewCommitteeId",
                        column: x => x.ReviewCommitteeId,
                        principalTable: "Review_Committees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Board_Member_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_Board_Member_ReviewCommitteeId",
                table: "Review_Board_Member",
                column: "ReviewCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Board_Member_ReviewerId",
                table: "Review_Board_Member",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Faculties_FacultyId",
                table: "Reviewers",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Faculties_FacultyId",
                table: "Reviewers");

            migrationBuilder.DropTable(
                name: "Review_Board_Member");

            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Competitions");

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                table: "Reviewers",
                newName: "DisciplineId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviewers_FacultyId",
                table: "Reviewers",
                newName: "IX_Reviewers_DisciplineId");

            migrationBuilder.RenameColumn(
                name: "IsSuccessed",
                table: "Acceptances",
                newName: "IsStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Disciplines_DisciplineId",
                table: "Reviewers",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "Id");
        }
    }
}
