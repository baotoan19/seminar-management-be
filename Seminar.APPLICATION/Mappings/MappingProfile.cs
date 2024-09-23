using AutoMapper;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.DOMAIN.Entitys;
using Seminar.APPLICATION.Models;
using Seminar.APPLICATION.Dtos.AccountDtos;
using Seminar.APPLICATION.Dtos.PostDto;
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
        }
    }
}