using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Models;
using Seminar.DOMAIN.Entitys;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces.IOrganizerService{
    public interface IOrganizerService{
        //Organizer
        Task<Organizer> CreateOrganizerAsync(CreateOrganizerDto createOrganizerDto);
        Task<OrganizerVM> GetOrganizerInforAsync(int id);
        Task<Organizer> UpdateOrganizerAsync(int id, UpdateOrganizerDto updateOrganizerDto);
        //Review Committee
        Task CreateReviewCommitteeAsync(CreateReviewCommitteeDto createReviewCommitteeDto);
        
    }
}