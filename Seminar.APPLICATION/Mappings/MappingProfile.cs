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
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Dtos.NotificationDtos;
using Seminar.APPLICATION.Dtos.CompetitionDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;
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
            CreateMap<ReviewBoardMemberDto, Reviewer>();
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
            CreateMap<Article, ArticleVM>()
            .ForMember(dest => dest.DisciplineName, opt => opt.MapFrom(src => src.Discipline.DisciplineName));
            CreateMap<CreateArticleDto, Article>();
            CreateMap<UpdateArticleDto, Article>();
            //Review Form
            CreateMap<Review_Form, ReviewFormVM>();
            CreateMap<CUReviewFormDto, Review_Form>();
            //Review Committee
            CreateMap<CreateReviewCommitteeDto, Review_Committee>()
            .ForMember(dest => dest.Review_Board_Members, opt => opt.Ignore());
            CreateMap<UpdateReviewCommitteeDto, Review_Committee>();
            //Registration Form
            CreateMap<RegistrationForm, RegistrationFormVM>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Author.AccountId))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.CompetitionName))
            .ForMember(dest => dest.InternalCode, opt => opt.MapFrom(src => src.Author.InternalCode))
            .ForMember(dest => dest.DateStart, opt => opt.MapFrom(src => src.Competition.DateStart))
            .ForMember(dest => dest.DateEnd, opt => opt.MapFrom(src => src.Competition.DateEnd));
            CreateMap<CreateRegistrationFormDto, RegistrationForm>();
            CreateMap<UpdateRegistrationFormDto, RegistrationForm>();
            //Notification
            CreateMap<CreateNotificationDto, Notification>();
            CreateMap<Notification, NotificationVM>();
            CreateMap<UpdateNotificationDto, Notification>();
            //Competition
            CreateMap<Competition, CompetitionVM>()
            .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.Name))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Organizer.AccountId));
            CreateMap<CreateCompetitionDto, Competition>();
            CreateMap<UpdateCompetitionDto, Competition>();
            //Research Topic
            CreateMap<CreateResearchTopicDto, ResearchTopic>();
            CreateMap<ResearchTopic, ResearchTopicVM>()
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Articles.Title))
            .ForMember(dest => dest.DisciplineName, opt => opt.MapFrom(src => src.Disciplines.DisciplineName))
            .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competitions.CompetitionName));
            //History Research Topic
            CreateMap<CreateHistoryResearchTopicDto, History_Update_ResearchTopic>();
            CreateMap<UpdateResearchTopicDto, ResearchTopic>();
            CreateMap<History_Update_ResearchTopic, HistoryUpdateResearchTopicVM>();
        }
    }
}