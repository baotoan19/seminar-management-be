using Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IResearchTopicService
{
    // Research Topic
    Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByCompetitionIdAsync(int competitionId, int reviewCommitteeId, int index, int pageSize, string nameTopicSearch, int disciplineId, int acceptedForPublicationStatus, int ReviewAcceptanceStatus);
    Task<ResearchTopicVM> GetResearchTopicByIdAsync(int id);
    Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByAuthorIdAsync(string roleName, int index, int pageSize, string nameTopicSearch, int acceptedForPublicationStatus, int ReviewAcceptanceStatus, int competitionId);
    Task<PaginatedList<ResearchTopicVM>> GetResearchTopicsForReviewAsync(int index, int pageSize, int idSearch, string nameTopicSearch, int isStatus, int competitionId);
    Task CreateResearchTopicAsync(CreateResearchTopicDto createResearchTopicDto);
    Task UpdateResearchTopicAsync(int researchTopicId, UpdateResearchTopicDto updateResearchTopicDto);
    Task UpdateIsReviewAcceptanceAsync(UpdateIsReviewAcceptanceDto updateIsReviewAcceptanceDto);
    Task UpdateDateEndResearchTopicAsync(int researchTopicId, UpdateDateEndResearchTopicDto updateDateEndResearchTopicDto);
    // History Research Topic
    Task<List<HistoryUpdateResearchTopicVM>> GetAllHistoryResearchTopicByResearchTopicIdAsync(int researchTopicId);
    Task CreateNewVersionResearchTopicAsync(CreateHistoryResearchTopicDto createHistoryResearchTopicDto);
    Task UpdateHistoryResearchTopicAsync(int historyResearchTopicId, UpdateHistoryResearchTopicDto updateHistoryResearchTopicDto);
    Task DeleteHistoryResearchTopicAsync(int historyResearchTopicId);
}