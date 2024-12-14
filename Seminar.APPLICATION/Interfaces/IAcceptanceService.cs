using Seminar.APPLICATION.Dtos.AcceptanceDtos;
using Seminar.APPLICATION.Dtos.ReviewAcceptanceDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IAcceptanceService
{
    Task<PaginatedList<AcceptanceVM>> GetAllAcceptances(int index, int pageSize, int idSearch, string nameSearch, int facultyAcceptedStatus, int acceptedForPublicationStatus, int competitionId, int facultyId,int accountId);
    Task<AcceptanceVM> GetAcceptanceById(int id);
    Task CreateAcceptance(CreateAcceptanceDto dto);
    Task DeleteAcceptance(int id);
    Task CreateReviewAcceptance(CreateReviewAcceptanceDto dto);
    Task<PaginatedList<ReviewAcceptanceVM>> GetAllReviewAcceptanceByAcceptanceId(int acceptanceId,int index, int pageSize);
    Task UpdateAcceptanceForPublication(int acceptanceId, UpdateAcceptanceForPublicationDto dto);
}