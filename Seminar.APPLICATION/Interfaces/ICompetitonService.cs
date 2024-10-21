using Seminar.APPLICATION.Dtos.CompetitionDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface ICompetitionService
{
    Task<PaginatedList<CompetitionVM>> GetAllCompetitionByOrganizerIdAsync(int index, int pageSize, string nameSearch);
    Task<PaginatedList<CompetitionVM>> GetAllCompetitionAsync(int index, int pageSize,string nameSearch,string organizerName);
    Task<CompetitionVM> GetCompetitionByIdAsync(int id);
    Task CreateCompetitionAsync(CreateCompetitionDto createCompetitionDto);
    Task UpdateCompetitionAsync(int id, UpdateCompetitionDto updateCompetitionDto);
    Task DeleteCompetitionAsync(int id);
}
