using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFacutiesAndReviewForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Faculty_FacultyId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizers_Faculty_FacultyId",
                table: "Organizers");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Forms_Articals_ArticalId",
                table: "Review_Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculty",
                table: "Faculty");

            migrationBuilder.RenameTable(
                name: "Faculty",
                newName: "Faculties");

            migrationBuilder.RenameColumn(
                name: "ArticalId",
                table: "Review_Forms",
                newName: "HistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_Forms_ArticalId",
                table: "Review_Forms",
                newName: "IX_Review_Forms_HistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculties",
                table: "Faculties",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Faculties_FacultyId",
                table: "Authors",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizers_Faculties_FacultyId",
                table: "Organizers",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Forms_History_Update_Articals_HistoryId",
                table: "Review_Forms",
                column: "HistoryId",
                principalTable: "History_Update_Articals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Faculties_FacultyId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizers_Faculties_FacultyId",
                table: "Organizers");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Forms_History_Update_Articals_HistoryId",
                table: "Review_Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculties",
                table: "Faculties");

            migrationBuilder.RenameTable(
                name: "Faculties",
                newName: "Faculty");

            migrationBuilder.RenameColumn(
                name: "HistoryId",
                table: "Review_Forms",
                newName: "ArticalId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_Forms_HistoryId",
                table: "Review_Forms",
                newName: "IX_Review_Forms_ArticalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculty",
                table: "Faculty",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Faculty_FacultyId",
                table: "Authors",
                column: "FacultyId",
                principalTable: "Faculty",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizers_Faculty_FacultyId",
                table: "Organizers",
                column: "FacultyId",
                principalTable: "Faculty",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Forms_Articals_ArticalId",
                table: "Review_Forms",
                column: "ArticalId",
                principalTable: "Articals",
                principalColumn: "Id");
        }
    }
}
