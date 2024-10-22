using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IRegistrationFormService
{
    public Task<PaginatedList<RegistrationFormVM>> GetAllByCompetitionIdAsync(int competitionId, int index, int pageSize, string idSearch, string internalCodeSearch, int isAccepted);
    public Task<List<RegistrationFormVM>> GetAllByAuthorIdAsync();
    public Task<RegistrationFormVM> GetRegistrationFormByIdAsync(int id);
    public Task CreateRegistrationFormAsync(CreateRegistrationFormDto dto);
    public Task UpdateRegistrationFormAsync(int id, UpdateRegistrationFormDto dto);
    public Task DeleteRegistrationFormAsync(int id);
}