using AutoMapper;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.DOMAIN.Entitys;
using Seminar.APPLICATION.Models;
using Seminar.APPLICATION.Dtos.AccountDtos;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Dtos.HistoryArticasDtos;
using Seminar.APPLICATION.Dtos.ProceedingsDtos;
using Seminar.APPLICATION.Dtos.ArticalsDtos;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
namespace Seminar.APPLICATION.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Accout
            CreateMap<RegisterRequestDto, Account>();
            CreateMap<Account, AccountVM>();
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
            CreateMap<PostDto, Post>();
            CreateMap<Post, PostVM>();
            //Conclude
            CreateMap<Conclude, ConcludeVM>();
            //Discipline
            CreateMap<Discipline, DisciplineVM>();
            //Faculty
            CreateMap<Faculty, FacultyVM>();
            //Artical
            //CreateMap<Artical, ArticalVM>();
            //CreateMap<CUArticalsDto, Artical>();
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
            CreateMap<RegistrationForm, RegistrationFormVM>();
            CreateMap<CURegistrationFormDto, RegistrationForm>();
        }
    }
}