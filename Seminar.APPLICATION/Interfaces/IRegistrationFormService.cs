using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IRegistrationFormService
{
    public Task<RegistrationFormVM> GetRegistrationFormByIdAsync(int id);
    public Task CreateRegistrationFormAsync(CURegistrationFormDto dto);
    public Task UpdateRegistrationFormAsync(int id, CURegistrationFormDto dto);
    public Task DeleteRegistrationFormAsync(int id);
}