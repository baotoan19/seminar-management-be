using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
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
        Task<PaginatedList<ReviewCommitteeVM>> GetReviewCommitteeByCompetitionIdAsync(int competitionId, int page, int pageSize, int idSearch, string nameSearch);
        Task CreateReviewCommitteeAsync(CreateReviewCommitteeDto createReviewCommitteeDto);
        //Review Assignment
        Task CreateReviewAssignmentAsync(CreateReviewAssignmentDto createReviewAssignmentDto);
        


    }
}