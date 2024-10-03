using Seminar.APPLICATION.Dtos.ProceedingsDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IProceedingService
{
    public Task<ProceedingVM> GetProceedingByIdAsync(int id);
    public Task CreateProceedingAsync(CUProceedingDto proceedingDto);
    public Task UpdateProceedingAsync(int id, CUProceedingDto proceedingDto);
    public Task DeleteProceedingAsync(int id);

}