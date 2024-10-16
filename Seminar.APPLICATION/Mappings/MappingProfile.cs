using AutoMapper;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.DOMAIN.Entitys;
using Seminar.APPLICATION.Models;
using Seminar.APPLICATION.Dtos.AccountDtos;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Dtos.NotificationDtos;
namespace Seminar.APPLICATION.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Accout
            CreateMap<RegisterRequestDto, Account>();
            CreateMap<Account, AccountVM>().ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<UpdateAccountDto, Account>();
            CreateMap<Account, ResponseAccountDto>();
            //Author
            CreateMap<CreateAuthorDto, Author>();
            CreateMap<Author, AuthorVM>();
            CreateMap<UpdateAuthorDto, Author>();
            //Organizers
            CreateMap<CreateOrganizerDto, Organizer>();
            CreateMap<Organizer, OrganizerVM>();
            CreateMap<UpdateOrganizerDto, Organizer>();
            //Reviewer
            CreateMap<CreateReviewerDto, Reviewer>();
            CreateMap<Reviewer, ReviewerVM>();
            CreateMap<UpdateReviewerDto, Reviewer>();
            //Post
            CreateMap<CreatePostDto, Post>();
            CreateMap<Post, PostVM>().ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizers.Name));
            CreateMap<UpdatePostDto, Post>();
            //Conclude
            CreateMap<Conclude, ConcludeVM>();
            //Discipline
            CreateMap<Discipline, DisciplineVM>();
            //Faculty
            CreateMap<Faculty, FacultyVM>();
            //Artical
            CreateMap<Article, ArticleVM>();
            CreateMap<CreateArticleDto, Article>();
            CreateMap<UpdateArticleDto, Article>();
            //History Update Artical
            //CreateMap<History_Update_Artical, HistoryArticalsVM>();
            //CreateMap<CreateHistoryArticalsDto, History_Update_Artical>();
            //Review Form
            CreateMap<Review_Form, ReviewFormVM>();
            CreateMap<CUReviewFormDto, Review_Form>();
            //Review Assignment
            CreateMap<Review_Assignment, ReviewAssignmentVM>();
            CreateMap<CUReviewAssignmentDto, Review_Assignment>();
            //Review Committee
            CreateMap<Review_Committee, ReviewCommitteeVM>();
            CreateMap<CUReviewCommitteeDto, Review_Committee>();
            //Registration Form
            CreateMap<RegistrationForm, RegistrationFormVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.CompetitionName));
            CreateMap<CreateRegistrationFormDto, RegistrationForm>();
            CreateMap<UpdateRegistrationFormDto, RegistrationForm>();
            //Notification
            CreateMap<CreateNotificationDto, Notification>();
            CreateMap<Notification, NotificationVM>();
            CreateMap<UpdateNotificationDto, Notification>();
        }
    }
}