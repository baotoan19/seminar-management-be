using AutoMapper;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.DOMAIN.Entitys;
namespace Seminar.APPLICATION.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Accout
            CreateMap<RegisterRequestDto, Account>();


            //Author
            CreateMap<CreateAuthorDto, Author>();


            //Organizers
            CreateMap<CreateOrganizerDto, Organizer>();


            //Reviewer
            CreateMap<CreateReviewerDto, Reviewer>();


            
        }
    }
}