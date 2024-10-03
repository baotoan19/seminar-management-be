using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IFacultyService
{
    Task<IList<FacultyVM>> GetAllFacultyAsync();
    Task<FacultyVM> GetFacultyByIdAsync(int id);
}