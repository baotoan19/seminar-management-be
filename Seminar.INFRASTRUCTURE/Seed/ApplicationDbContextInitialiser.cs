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
                await addOrganizer();
                await addFaculty();
                await addDiscipline();
                await addConference();
                await addReviewCommittee();
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
                    new Role { RoleName = CLAIMS_VALUES.ROLE_TYPE.AUTHOR },
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
                var organizerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.ORGANIZER);
                if (organizerRole != null)
                {
                    var account = new Account
                    {
                        Email = "admin@gmail.com",
                        Password = passwordHasher.HashPassword(null, "Admin@123"),
                        RoleId = organizerRole.Id,
                        Status = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        private async Task addOrganizer()
        {
            if (!await _context.Organizers.AnyAsync(x => x.DeletedAt == null))
            {
                var adminAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == "admin@gmail.com");
                if (adminAccount != null)
                {
                    var organizer = new Organizer
                    {
                        Name = "Admin",
                        NumberPhone = "0372673566",
                        AccountId = adminAccount.Id,
                        FacultyId = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _unitOfWork.GetRepository<Organizer>().InsertAsync(organizer);
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

        private async Task addConference()
        {
            if (!await _context.Conferences.AnyAsync(x => x.DeletedAt == null))
            {
                Conference[] conferences =
                [
                    new Conference { ConferenceName = "Hội nghị khoa học thực phẩm" , DateStart = new DateTime(2024, 06, 01), DateEnd = new DateTime(2024, 07, 02) , Destination = "Hồ Chí Minh" , OrganizerId = 1 },
                    new Conference { ConferenceName = "Hội nghị khoa học và công nghệ quốc tế" , DateStart = new DateTime(2024, 09, 01), DateEnd = new DateTime(2024, 10, 02) , Destination = "Hồ Chí Minh" , OrganizerId = 2 }
                ];
                foreach (Conference conference in conferences)
                {
                    if (!await _unitOfWork.GetRepository<Conference>().Entities.AnyAsync(c => c.ConferenceName == conference.ConferenceName))
                    {
                        conference.CreatedAt = DateTime.Now;
                        conference.UpdatedAt = DateTime.Now;
                        await _unitOfWork.GetRepository<Conference>().InsertAsync(conference);
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
                    new Review_Committee { ReviewCommitteeName = "Hội đồng 12DHTH01", ConferenceId = 1 },
                    new Review_Committee { ReviewCommitteeName = "Hội đồng 12DHTH02", ConferenceId = 2 }
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


    }
}