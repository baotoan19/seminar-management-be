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
    }
}