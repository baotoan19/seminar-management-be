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
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Review_Committees_Review_CommitteeId",
                table: "Reviewers");

            migrationBuilder.DropColumn(
                name: "ReviewCommitteeId",
                table: "Reviewers");

            migrationBuilder.AlterColumn<int>(
                name: "Review_CommitteeId",
                table: "Reviewers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Review_Committees_Review_CommitteeId",
                table: "Reviewers",
                column: "Review_CommitteeId",
                principalTable: "Review_Committees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Review_Committees_Review_CommitteeId",
                table: "Reviewers");

            migrationBuilder.AlterColumn<int>(
                name: "Review_CommitteeId",
                table: "Reviewers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCommitteeId",
                table: "Reviewers",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Review_Committees_Review_CommitteeId",
                table: "Reviewers",
                column: "Review_CommitteeId",
                principalTable: "Review_Committees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
