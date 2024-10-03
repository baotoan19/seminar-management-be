using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IDisciplineService
{
    Task<IList<DisciplineVM>> GetAllDisciplineAsync();
    Task<DisciplineVM> GetDisciplineByIdAsync(int id);
}
