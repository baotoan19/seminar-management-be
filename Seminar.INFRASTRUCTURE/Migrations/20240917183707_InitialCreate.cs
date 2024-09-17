using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seminar.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conclude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Result = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conclude", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisciplineName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faculty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proceedings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proceedings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    InternalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacultyId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_Faculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculty",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Organizers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    FacultyId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Organizers_Faculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculty",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Conferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConferenceName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUpload = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Organizers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Organizers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Articals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUpload = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisciplineId = table.Column<int>(type: "int", nullable: true),
                    ConferenceId = table.Column<int>(type: "int", nullable: true),
                    ProceedingId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articals_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articals_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articals_Proceedings_ProceedingId",
                        column: x => x.ProceedingId,
                        principalTable: "Proceedings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegistrationForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    ConferenceId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAccepted = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationForms_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationForms_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review_Committees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewCommitteeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ConferenceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_Committees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Committees_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Author_Articlas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    ArticalId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author_Articlas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Author_Articlas_Articals_ArticalId",
                        column: x => x.ArticalId,
                        principalTable: "Articals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Author_Articlas_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "History_Update_Articals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticalId = table.Column<int>(type: "int", nullable: true),
                    NewFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_Update_Articals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Update_Articals_Articals_ArticalId",
                        column: x => x.ArticalId,
                        principalTable: "Articals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticalId = table.Column<int>(type: "int", nullable: true),
                    NotificationContent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NotificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecevierId = table.Column<int>(type: "int", nullable: true),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Articals_ArticalId",
                        column: x => x.ArticalId,
                        principalTable: "Articals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcademicRank = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AcademicDegree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisciplineId = table.Column<int>(type: "int", nullable: true),
                    ReviewCommitteeId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviewers_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviewers_Review_Committees_ReviewCommitteeId",
                        column: x => x.ReviewCommitteeId,
                        principalTable: "Review_Committees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Review_Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    ArticalId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Assignments_Articals_ArticalId",
                        column: x => x.ArticalId,
                        principalTable: "Articals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Review_Assignments_Organizers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Organizers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Review_Assignments_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Review_Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticalId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    ConcludeId = table.Column<int>(type: "int", nullable: true),
                    Date_Upload = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_Forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Forms_Articals_ArticalId",
                        column: x => x.ArticalId,
                        principalTable: "Articals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Review_Forms_Conclude_ConcludeId",
                        column: x => x.ConcludeId,
                        principalTable: "Conclude",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Review_Forms_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articals_ConferenceId",
                table: "Articals",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Articals_DisciplineId",
                table: "Articals",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Articals_ProceedingId",
                table: "Articals",
                column: "ProceedingId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Articlas_ArticalId",
                table: "Author_Articlas",
                column: "ArticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_Articlas_AuthorId",
                table: "Author_Articlas",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_AccountId",
                table: "Authors",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_FacultyId",
                table: "Authors",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Conferences_OrganizerId",
                table: "Conferences",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_History_Update_Articals_ArticalId",
                table: "History_Update_Articals",
                column: "ArticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ArticalId",
                table: "Notifications",
                column: "ArticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizers_AccountId",
                table: "Organizers",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizers_FacultyId",
                table: "Organizers",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OrganizerId",
                table: "Posts",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationForms_AuthorId",
                table: "RegistrationForms",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationForms_ConferenceId",
                table: "RegistrationForms",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Assignments_ArticalId",
                table: "Review_Assignments",
                column: "ArticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Assignments_OrganizerId",
                table: "Review_Assignments",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Assignments_ReviewerId",
                table: "Review_Assignments",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Committees_ConferenceId",
                table: "Review_Committees",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Forms_ArticalId",
                table: "Review_Forms",
                column: "ArticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Forms_ConcludeId",
                table: "Review_Forms",
                column: "ConcludeId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Forms_ReviewerId",
                table: "Review_Forms",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_AccountId",
                table: "Reviewers",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_DisciplineId",
                table: "Reviewers",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_ReviewCommitteeId",
                table: "Reviewers",
                column: "ReviewCommitteeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Author_Articlas");

            migrationBuilder.DropTable(
                name: "History_Update_Articals");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "RegistrationForms");

            migrationBuilder.DropTable(
                name: "Review_Assignments");

            migrationBuilder.DropTable(
                name: "Review_Forms");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Articals");

            migrationBuilder.DropTable(
                name: "Conclude");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropTable(
                name: "Proceedings");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Review_Committees");

            migrationBuilder.DropTable(
                name: "Conferences");

            migrationBuilder.DropTable(
                name: "Organizers");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Faculty");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
