using Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IResearchTopicService
{
    // Research Topic
    Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByCompetitionIdAsync(int competitionId, int index, int pageSize, string nameTopicSearch, int disciplineId);
    Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByAuthorIdAsync(string roleName, int index, int pageSize, string nameTopicSearch);
    Task CreateResearchTopicAsync(CreateResearchTopicDto createResearchTopicDto);
    Task UpdateResearchTopicAsync(int researchTopicId, UpdateResearchTopicDto updateResearchTopicDto);
    Task<ResearchTopicVM> GetResearchTopicByIdAsync(int id);
    Task<List<ResearchTopicAuthorVM>> GetAuthorByResearchTopicIdAsync(int id);
    Task UpdateIsAcceptanceApprovedAsync(UpdateIsAcceptanceApprovedDto updateIsAcceptanceApprovedDto);
    Task UpdateIsReviewAcceptanceAsync(UpdateIsReviewAcceptanceDto updateIsReviewAcceptanceDto);
    // History Research Topic
    Task CreateNewVersionResearchTopicAsync(CreateHistoryResearchTopicDto createHistoryResearchTopicDto);
    Task<List<HistoryUpdateResearchTopicVM>> GetAllHistoryResearchTopicByResearchTopicIdAsync(int researchTopicId);
    
}