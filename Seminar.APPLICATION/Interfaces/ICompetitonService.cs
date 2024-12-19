using Seminar.APPLICATION.Dtos.CompetitionDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface ICompetitionService
{
    Task<PaginatedList<CompetitionVM>> GetAllCompetitionByOrganizerIdAsync(int index, int pageSize, string nameSearch,int year);
    Task<PaginatedList<CompetitionVM>> GetAllCompetitionAsync(int index, int pageSize, string nameSearch, string organizerName, int facultyId, int year);
    Task<CompetitionVM> GetCompetitionByIdAsync(int id);
    Task CreateCompetitionAsync(CreateCompetitionDto createCompetitionDto);
    Task UpdateCompetitionAsync(int id, UpdateCompetitionDto updateCompetitionDto);
    Task DeleteCompetitionAsync(int id);
    Task UpdateDateEndCompetitionAsync(int id, UpdateDateEndCompetitionDto updateDateEndCompetitionDto);
    Task UpdateDateSubmitCompetitionAsync(int id, UpdateDateSubmitCompetitionDto updateDateSubmitCompetitionDto);
}
