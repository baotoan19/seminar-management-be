using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Review_Committees_Review_CommitteeId",
                table: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_Reviewers_Review_CommitteeId",
                table: "Reviewers");

            migrationBuilder.DropColumn(
                name: "Review_CommitteeId",
                table: "Reviewers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Review_CommitteeId",
                table: "Reviewers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_Review_CommitteeId",
                table: "Reviewers",
                column: "Review_CommitteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Review_Committees_Review_CommitteeId",
                table: "Reviewers",
                column: "Review_CommitteeId",
                principalTable: "Review_Committees",
                principalColumn: "Id");
        }
    }
}
