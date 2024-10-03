using Seminar.APPLICATION.Dtos.ArticalsDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IArticalsService
{
    Task<ArticalVM> GetArticalByIdAsync(int id);
    Task CreateArticalAsync(CUArticalsDto createArticalsDto);
    Task UpdateArticalAsync(int id, CUArticalsDto updateArticalsDto);
    Task DeleteArticalAsync(int id);
}