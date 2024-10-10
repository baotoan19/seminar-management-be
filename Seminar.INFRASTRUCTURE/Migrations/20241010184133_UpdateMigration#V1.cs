using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Articels_ArticelId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ArticelId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ArticelId",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticelId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ArticelId",
                table: "Notifications",
                column: "ArticelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Articels_ArticelId",
                table: "Notifications",
                column: "ArticelId",
                principalTable: "Articels",
                principalColumn: "Id");
        }
    }
}
