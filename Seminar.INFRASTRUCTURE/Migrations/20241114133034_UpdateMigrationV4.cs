using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptanceApproved",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "IsReviewAcceptance",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "IsAcceptedForPublication",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "IsAcceptedForPublication",
                table: "Acceptances");

            migrationBuilder.DropColumn(
                name: "IsFacultyAccepted",
                table: "Acceptances");

            migrationBuilder.AddColumn<int>(
                name: "AcceptanceApprovedStatus",
                table: "ResearchTopics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewAcceptanceStatus",
                table: "ResearchTopics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcceptedForPublicationStatus",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcceptedForPublicationStatus",
                table: "Acceptances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FacultyAcceptedStatus",
                table: "Acceptances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceApprovedStatus",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "ReviewAcceptanceStatus",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "AcceptedForPublicationStatus",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AcceptedForPublicationStatus",
                table: "Acceptances");

            migrationBuilder.DropColumn(
                name: "FacultyAcceptedStatus",
                table: "Acceptances");

            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptanceApproved",
                table: "ResearchTopics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewAcceptance",
                table: "ResearchTopics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedForPublication",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedForPublication",
                table: "Acceptances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFacultyAccepted",
                table: "Acceptances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
