using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Seminar.APPLICATION.Services;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Database;
using Seminar.INFRASTRUCTURE.Repositories;
using Seminar.INFRASTRUCTURE.UnitOfWork;

namespace Seminar.APPLICATION.Extensions
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepository();
            services.AddService(configuration);
            services.AddAutoMapper();
        }


        //Đăng ký repository
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        //Đăng ký service
        public static void AddService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddScoped(typeof(ITokenService), typeof(TokenService));
            services.AddScoped(typeof(IAuthorService), typeof(AuthorService));
            services.AddScoped(typeof(IOrganizerService), typeof(OrganizerService));
            services.AddScoped(typeof(IReviewerService), typeof(ReviewerService));
            services.AddScoped(typeof(IAccountService), typeof(AccountService));
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(IPostService), typeof(PostService));
            services.AddScoped(typeof(IConcludeService), typeof(ConcludeService));
            services.AddScoped(typeof(IDisciplineService), typeof(DisciplineService));
            services.AddScoped(typeof(IFacultyService), typeof(FacultyService));
            services.AddScoped(typeof(IProceedingService), typeof(ProceedingService));
            services.AddScoped(typeof(IArticalsService), typeof(ArticalService));
            services.AddScoped(typeof(IHistoryArticalsService), typeof(HistoryArticalsService));
            services.AddScoped(typeof(IReviewFormService), typeof(ReviewFormService));
            services.AddScoped(typeof(IReviewAssignmentService), typeof(ReviewAssignmentService));
            services.AddScoped(typeof(IReviewCommitteeService), typeof(ReviewCommitteeService));
            services.AddScoped(typeof(IRegistrationFormService), typeof(RegistrationFormService));
        }

        //Đăng ký mapper
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}