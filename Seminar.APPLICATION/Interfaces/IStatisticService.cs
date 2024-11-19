using Seminar.APPLICATION.Dtos.StatisticDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IStatisticService
{
    Task<StatisticsVM> GetStatisticsByOrganizer(StatisticsFilterDto filterDto);
}
