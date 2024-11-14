using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Review_Acceptances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Review_Acceptances");
        }
    }
}
