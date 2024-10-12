using System;
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
            migrationBuilder.DropForeignKey(
                name: "FK_Acceptances_Topics_TopicId",
                table: "Acceptances");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Topics_TopicId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationForms_Conferences_ConferenceId",
                table: "RegistrationForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Assignments_Topics_TopicId",
                table: "Review_Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Committees_Conferences_ConferenceId",
                table: "Review_Committees");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Forms_History_Update_Topics_HistoryId",
                table: "Review_Forms");

            migrationBuilder.DropTable(
                name: "Author_Articels");

            migrationBuilder.DropTable(
                name: "Author_Topics");

            migrationBuilder.DropTable(
                name: "History_Update_Topics");

            migrationBuilder.DropTable(
                name: "Articels");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Conferences");

            migrationBuilder.DropIndex(
                name: "IX_Review_Forms_HistoryId",
                table: "Review_Forms");

            migrationBuilder.RenameColumn(
                name: "ConferenceId",
                table: "Review_Committees",
                newName: "CompetitionId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_Committees_ConferenceId",
                table: "Review_Committees",
                newName: "IX_Review_Committees_CompetitionId");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "Review_Assignments",
                newName: "ResearchTopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_Assignments_TopicId",
                table: "Review_Assignments",
                newName: "IX_Review_Assignments_ResearchTopicId");

            migrationBuilder.RenameColumn(
                name: "ConferenceId",
                table: "RegistrationForms",
                newName: "CompetitionId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationForms_ConferenceId",
                table: "RegistrationForms",
                newName: "IX_RegistrationForms_CompetitionId");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "Notifications",
                newName: "ResearchTopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TopicId",
                table: "Notifications",
                newName: "IX_Notifications_ResearchTopicId");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "Acceptances",
                newName: "ResearchTopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Acceptances_TopicId",
                table: "Acceptances",
                newName: "IX_Acceptances_ResearchTopicId");

            migrationBuilder.AddColumn<int>(
                name: "History_Update_ResearchTopicId",
                table: "Review_Forms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUpload = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsStatus = table.Column<bool>(type: "bit", nullable: false),
                    DisciplineId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizerId = table.Column<int>(type: "int", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Competitions_Organizers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Organizers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Author_Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    ArticleId = table.Column<int>(type: "int", nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Author_Articles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Author_Articles_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ResearchTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameTopic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpLoad = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Budget = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AchievedResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAcceptanceStatus = table.Column<bool>(type: "bit", nullable: false),
                    IsReviewStatus = table.Column<bool>(type: "bit", nullable: false),
                    ProductFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleId = table.Column<int>(type: "int", nullable: true),
                    DisciplineId = table.Column<int>(type: "int", nullable: false),
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResearchTopics_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResearchTopics_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Author_ResearchTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResearchTopicId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author_ResearchTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Author_ResearchTopics_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Author_ResearchTopics_ResearchTopics_ResearchTopicId",
                        column: x => x.ResearchTopicId,
                        principalTable: "ResearchTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History_Update_ResearchTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResearchTopicId = table.Column<int>(type: "int", nullable: true),
                    NewFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_Update_ResearchTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Update_ResearchTopics_ResearchTopics_ResearchTopicId",
                        column: x => x.ResearchTopicId,
                        principalTable: "ResearchTopics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_Forms_History_Update_ResearchTopicId",
                table: "Review_Forms",
                column: "History_Update_ResearchTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DisciplineId",
                table: "Articles",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Articles_ArticleId",
                table: "Author_Articles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Articles_AuthorId",
                table: "Author_Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_ResearchTopics_AuthorId",
                table: "Author_ResearchTopics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_ResearchTopics_ResearchTopicId",
                table: "Author_ResearchTopics",
                column: "ResearchTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_OrganizerId",
                table: "Competitions",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_History_Update_ResearchTopics_ResearchTopicId",
                table: "History_Update_ResearchTopics",
                column: "ResearchTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchTopics_CompetitionId",
                table: "ResearchTopics",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchTopics_DisciplineId",
                table: "ResearchTopics",
                column: "DisciplineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptances_ResearchTopics_ResearchTopicId",
                table: "Acceptances",
                column: "ResearchTopicId",
                principalTable: "ResearchTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ResearchTopics_ResearchTopicId",
                table: "Notifications",
                column: "ResearchTopicId",
                principalTable: "ResearchTopics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationForms_Competitions_CompetitionId",
                table: "RegistrationForms",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Assignments_ResearchTopics_ResearchTopicId",
                table: "Review_Assignments",
                column: "ResearchTopicId",
                principalTable: "ResearchTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Committees_Competitions_CompetitionId",
                table: "Review_Committees",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Forms_History_Update_ResearchTopics_History_Update_ResearchTopicId",
                table: "Review_Forms",
                column: "History_Update_ResearchTopicId",
                principalTable: "History_Update_ResearchTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acceptances_ResearchTopics_ResearchTopicId",
                table: "Acceptances");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ResearchTopics_ResearchTopicId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationForms_Competitions_CompetitionId",
                table: "RegistrationForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Assignments_ResearchTopics_ResearchTopicId",
                table: "Review_Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Committees_Competitions_CompetitionId",
                table: "Review_Committees");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Forms_History_Update_ResearchTopics_History_Update_ResearchTopicId",
                table: "Review_Forms");

            migrationBuilder.DropTable(
                name: "Author_Articles");

            migrationBuilder.DropTable(
                name: "Author_ResearchTopics");

            migrationBuilder.DropTable(
                name: "History_Update_ResearchTopics");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "ResearchTopics");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropIndex(
                name: "IX_Review_Forms_History_Update_ResearchTopicId",
                table: "Review_Forms");

            migrationBuilder.DropColumn(
                name: "History_Update_ResearchTopicId",
                table: "Review_Forms");

            migrationBuilder.RenameColumn(
                name: "CompetitionId",
                table: "Review_Committees",
                newName: "ConferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_Committees_CompetitionId",
                table: "Review_Committees",
                newName: "IX_Review_Committees_ConferenceId");

            migrationBuilder.RenameColumn(
                name: "ResearchTopicId",
                table: "Review_Assignments",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_Assignments_ResearchTopicId",
                table: "Review_Assignments",
                newName: "IX_Review_Assignments_TopicId");

            migrationBuilder.RenameColumn(
                name: "CompetitionId",
                table: "RegistrationForms",
                newName: "ConferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationForms_CompetitionId",
                table: "RegistrationForms",
                newName: "IX_RegistrationForms_ConferenceId");

            migrationBuilder.RenameColumn(
                name: "ResearchTopicId",
                table: "Notifications",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ResearchTopicId",
                table: "Notifications",
                newName: "IX_Notifications_TopicId");

            migrationBuilder.RenameColumn(
                name: "ResearchTopicId",
                table: "Acceptances",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Acceptances_ResearchTopicId",
                table: "Acceptances",
                newName: "IX_Acceptances_TopicId");

            migrationBuilder.CreateTable(
                name: "Articels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisciplineId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpload = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStatus = table.Column<bool>(type: "bit", nullable: false),
                    KeyWord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articels_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Conferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    ConferenceName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conferences_Organizers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Organizers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Author_Articels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticelId = table.Column<int>(type: "int", nullable: true),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author_Articels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Author_Articels_Articels_ArticelId",
                        column: x => x.ArticelId,
                        principalTable: "Articels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Author_Articels_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConferenceId = table.Column<int>(type: "int", nullable: false),
                    DisciplineId = table.Column<int>(type: "int", nullable: false),
                    AchievedResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArticelId = table.Column<int>(type: "int", nullable: true),
                    Budget = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpLoad = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAcceptanceStatus = table.Column<bool>(type: "bit", nullable: false),
                    IsReviewStatus = table.Column<bool>(type: "bit", nullable: false),
                    NameTopic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Topics_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Author_Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Author_Topics_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Author_Topics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History_Update_Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_Update_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Update_Topics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_Forms_HistoryId",
                table: "Review_Forms",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articels_DisciplineId",
                table: "Articels",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Articels_ArticelId",
                table: "Author_Articels",
                column: "ArticelId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Articels_AuthorId",
                table: "Author_Articels",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Topics_AuthorId",
                table: "Author_Topics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Topics_TopicId",
                table: "Author_Topics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Conferences_OrganizerId",
                table: "Conferences",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_History_Update_Topics_TopicId",
                table: "History_Update_Topics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ConferenceId",
                table: "Topics",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_DisciplineId",
                table: "Topics",
                column: "DisciplineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Acceptances_Topics_TopicId",
                table: "Acceptances",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Topics_TopicId",
                table: "Notifications",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationForms_Conferences_ConferenceId",
                table: "RegistrationForms",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Assignments_Topics_TopicId",
                table: "Review_Assignments",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Committees_Conferences_ConferenceId",
                table: "Review_Committees",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Forms_History_Update_Topics_HistoryId",
                table: "Review_Forms",
                column: "HistoryId",
                principalTable: "History_Update_Topics",
                principalColumn: "Id");
        }
    }
}
