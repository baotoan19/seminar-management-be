using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalFilePath",
                table: "Acceptances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinalFilePath",
                table: "Acceptances",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
