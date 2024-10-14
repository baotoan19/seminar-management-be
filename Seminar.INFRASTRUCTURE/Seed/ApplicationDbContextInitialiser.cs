using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seminar.CORE.Base;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Database;

namespace Seminar.INFRASTRUCTURE.Seed
{
    public class ApplicationDbContextInitialiser
    {
        private readonly SeminarContext _context;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationDbContextInitialiser(SeminarContext context, ILogger<ApplicationDbContextInitialiser> logger, IUnitOfWork unitOfWork)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    Boolean dbExist = await _context.Database.CanConnectAsync();
                    if (!dbExist)
                    {
                        await _context.Database.EnsureCreatedAsync();
                        await _context.Database.MigrateAsync();
                    }
                    else
                    {
                        await _context.Database.MigrateAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await addRole();
                await addAccount();
                await addFaculty();
                await addOrganizer();
                await addCompetition();
                await addDiscipline();
                await addReviewCommittee();
                await addConclude();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task addRole()
        {
            if (!await _context.Roles.AnyAsync(x => x.DeletedAt == null))
            {
                Role[] roles =
                [
                    new Role { RoleName = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN},
                    new Role { RoleName = CLAIMS_VALUES.ROLE_TYPE.AUTHOR },
                    new Role { RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR },
                    new Role { RoleName = CLAIMS_VALUES.ROLE_TYPE.REVIEWER },
                    new Role { RoleName = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER },
                ];

                foreach (Role role in roles)
                {
                    if (!await _unitOfWork.GetRepository<Role>().Entities.AnyAsync(r => r.RoleName == role.RoleName))
                    {
                        role.CreatedAt = DateTime.Now;
                        role.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Role>().InsertAsync(role);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task addAccount()
        {
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            if (!await _context.Accounts.AnyAsync(x => x.DeletedAt == null))
            {
                var supperAdminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN);
                var organizerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.ORGANIZER);

                if (supperAdminRole != null && organizerRole != null)
                {
                    Account[] accounts =
                    [
                        new Account
                {
                    Email = "admin@gmail.com",
                    Password = passwordHasher.HashPassword(null, "Admin@123"),
                    RoleId = supperAdminRole.Id,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Account
                {
                    Email = "khoacntt@gmail.com",
                    Password = passwordHasher.HashPassword(null, "Khoacntt@123"),
                    RoleId = organizerRole.Id,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Account
                {
                    Email = "khoacntp@gmail.com",
                    Password = passwordHasher.HashPassword(null, "Khoacntp@123"),
                    RoleId = organizerRole.Id,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                        }
                    ];

                    foreach (var account in accounts)
                    {
                        await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }


        private async Task addOrganizer()
        {
            if (!await _context.Organizers.AnyAsync(x => x.DeletedAt == null))
            {
                var organizerAccounts = await _context.Accounts
                    .Where(a => a.Email == "khoacntt@gmail.com" || a.Email == "khoacntp@gmail.com")
                    .ToListAsync();

                if (organizerAccounts.Count == 2)
                {
                    Organizer[] organizers =
                    [
                        new Organizer
                {
                    Name = "Khoa CNTT",
                    NumberPhone = "0937829271",
                    AccountId = organizerAccounts[0].Id,
                    FacultyId = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Organizer
                {
                    Name = "Khoa CNTP",
                    NumberPhone = "0938182666",
                    AccountId = organizerAccounts[1].Id,
                    FacultyId = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
                    ];

                    foreach (var organizer in organizers)
                    {
                        await _unitOfWork.GetRepository<Organizer>().InsertAsync(organizer);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        private async Task addFaculty()
        {
            if (!await _context.Faculties.AnyAsync(x => x.DeletedAt == null))
            {
                Faculty[] faculties =
                [
                    new Faculty { FacultyName = "Công Nghệ Thông Tin" , Description = "HUIT" },
                    new Faculty { FacultyName = "Công Nghệ Sinh Học" , Description = "HUIT" },
                    new Faculty { FacultyName = "Điện - Điện Tử" , Description = "HUIT" },
                    new Faculty { FacultyName = "Quản Trị Khinh Doanh" , Description = "HUIT" },
                    new Faculty { FacultyName = "Công Nghệ Thực Phẩm" , Description = "HUIT" }
                ];

                foreach (Faculty faculty in faculties)
                {
                    if (!await _unitOfWork.GetRepository<Faculty>().Entities.AnyAsync(f => f.FacultyName == faculty.FacultyName))
                    {
                        faculty.CreatedAt = DateTime.Now;
                        faculty.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Faculty>().InsertAsync(faculty);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task addDiscipline()
        {
            if (!await _context.Disciplines.AnyAsync(x => x.DeletedAt == null))
            {
                Discipline[] disciplines =
                [
                    new Discipline { DisciplineName = "Trí Tuệ Nhân Tạo" },
                    new Discipline { DisciplineName = "Công nghệ sinh học công nghiệp"},
                    new Discipline { DisciplineName = "Kỹ thuật điện dân dụng"},
                    new Discipline { DisciplineName = "Kinh tế và kinh doanh"},
                    new Discipline { DisciplineName = "Kỹ thuật thực phẩm và đồ uống"}
                ];

                foreach (Discipline discipline in disciplines)
                {
                    if (!await _unitOfWork.GetRepository<Discipline>().Entities.AnyAsync(d => d.DisciplineName == discipline.DisciplineName))
                    {
                        discipline.CreatedAt = DateTime.Now;
                        discipline.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Discipline>().InsertAsync(discipline);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task addCompetition()
        {
            if (!await _context.Competitions.AnyAsync(x => x.DeletedAt == null))
            {
                Competition[] competitions =
                [
                    new Competition { CompetitionName = "Cuộc thi nghiên cứu khoa học sinh viên khoa CNTT lần thứ 1" , DateStart = new DateTime(2024, 06, 01), DateEnd = new DateTime(2024, 07, 02) , OrganizerId = 1 },
                    new Competition { CompetitionName = "Cuộc thi nghiên cứu khoa học sinh viên khoa CNTP lần thứ 1" , DateStart = new DateTime(2024, 09, 09), DateEnd = new DateTime(2024, 10, 10) , OrganizerId = 2 },
                ];
                foreach (Competition competition in competitions)
                {
                    if (!await _unitOfWork.GetRepository<Competition>().Entities.AnyAsync(c => c.CompetitionName == competition.CompetitionName))
                    {
                        competition.CreatedAt = DateTime.Now;
                        competition.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Competition>().InsertAsync(competition);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task addReviewCommittee()
        {
            if (!await _context.Review_Committees.AnyAsync(x => x.DeletedAt == null))
            {
                Review_Committee[] review_Committees =
                [
                    new Review_Committee { ReviewCommitteeName = "Hội đồng khoa CNTT", CompetitionId = 1 },
                    new Review_Committee { ReviewCommitteeName = "Hội đồng khoa CNTP", CompetitionId = 2 }
                ];
                foreach (Review_Committee review_Committee in review_Committees)
                {
                    if (!await _unitOfWork.GetRepository<Review_Committee>().Entities.AnyAsync(c => c.ReviewCommitteeName == review_Committee.ReviewCommitteeName))
                    {
                        review_Committee.CreatedAt = DateTime.Now;
                        review_Committee.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Review_Committee>().InsertAsync(review_Committee);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task addConclude()
        {
            if (!await _context.Concludes.AnyAsync(x => x.DeletedAt == null))
            {
                Conclude[] concludes =
                [
                    new Conclude {Result = "Accepted"},
                    new Conclude {Result = "Rejected"},
                    new Conclude {Result = "Revision Required"}
                ];

                foreach (Conclude conclude in concludes)
                {
                    if (!await _unitOfWork.GetRepository<Conclude>().Entities.AnyAsync(c => c.Result == conclude.Result))
                    {
                        conclude.CreatedAt = DateTime.Now;
                        conclude.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Conclude>().InsertAsync(conclude);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}