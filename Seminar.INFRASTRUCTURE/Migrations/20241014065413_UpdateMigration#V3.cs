using System;
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
            migrationBuilder.DropForeignKey(
                name: "FK_History_Update_ResearchTopics_ResearchTopics_ResearchTopicId",
                table: "History_Update_ResearchTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ResearchTopics_ResearchTopicId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ResearchTopicId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "FinalFilePath",
                table: "ResearchTopics");

            migrationBuilder.DropColumn(
                name: "ResearchTopicId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewFilePath",
                table: "History_Update_ResearchTopics");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "Review_Assignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "Review_Assignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RecevierId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NotificationDate",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NotificationContent",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationTypeId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ResearchTopicId",
                table: "History_Update_ResearchTopics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewProductFilePath",
                table: "History_Update_ResearchTopics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewReportFilePath",
                table: "History_Update_ResearchTopics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Budget",
                table: "Acceptances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

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

            migrationBuilder.AddColumn<string>(
                name: "FinalFilePath",
                table: "Acceptances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchTopics_ArticleId",
                table: "ResearchTopics",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Update_ResearchTopics_ResearchTopics_ResearchTopicId",
                table: "History_Update_ResearchTopics",
                column: "ResearchTopicId",
                principalTable: "ResearchTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTypes_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchTopics_Articles_ArticleId",
                table: "ResearchTopics",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_Update_ResearchTopics_ResearchTopics_ResearchTopicId",
                table: "History_Update_ResearchTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTypes_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ResearchTopics_Articles_ArticleId",
                table: "ResearchTopics");

            migrationBuilder.DropTable(
                name: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_ResearchTopics_ArticleId",
                table: "ResearchTopics");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Review_Assignments");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "Review_Assignments");

            migrationBuilder.DropColumn(
                name: "NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewProductFilePath",
                table: "History_Update_ResearchTopics");

            migrationBuilder.DropColumn(
                name: "NewReportFilePath",
                table: "History_Update_ResearchTopics");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Acceptances");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Acceptances");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "Acceptances");

            migrationBuilder.DropColumn(
                name: "FinalFilePath",
                table: "Acceptances");

            migrationBuilder.AddColumn<double>(
                name: "Budget",
                table: "ResearchTopics",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "ResearchTopics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "ResearchTopics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FinalFilePath",
                table: "ResearchTopics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RecevierId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NotificationDate",
                table: "Notifications",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationContent",
                table: "Notifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ResearchTopicId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ResearchTopicId",
                table: "History_Update_ResearchTopics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NewFilePath",
                table: "History_Update_ResearchTopics",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ResearchTopicId",
                table: "Notifications",
                column: "ResearchTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Update_ResearchTopics_ResearchTopics_ResearchTopicId",
                table: "History_Update_ResearchTopics",
                column: "ResearchTopicId",
                principalTable: "ResearchTopics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ResearchTopics_ResearchTopicId",
                table: "Notifications",
                column: "ResearchTopicId",
                principalTable: "ResearchTopics",
                principalColumn: "Id");
        }
    }
}
