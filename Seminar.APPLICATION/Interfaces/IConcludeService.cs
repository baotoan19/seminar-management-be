using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IConcludeService
{
    Task<IList<ConcludeVM>> GetAllAsync();
    Task<ConcludeVM> GetByIdAsync(int id);
}
