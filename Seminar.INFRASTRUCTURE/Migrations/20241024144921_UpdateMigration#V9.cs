using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationV9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Acceptances");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "Acceptances");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "ResearchTopics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "ResearchTopics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Review_Acceptances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcceptanceId = table.Column<int>(type: "int", nullable: false),
                    OrganizerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_Acceptances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Acceptances_Acceptances_AcceptanceId",
                        column: x => x.AcceptanceId,
                        principalTable: "Acceptances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Acceptances_Organizers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Organizers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_Acceptances_AcceptanceId",
                table: "Review_Acceptances",
                column: "AcceptanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Acceptances_OrganizerId",
                table: "Review_Acceptances",
                column: "OrganizerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Review_Acceptances");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "ResearchTopics");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "Acceptances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "Acceptances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
