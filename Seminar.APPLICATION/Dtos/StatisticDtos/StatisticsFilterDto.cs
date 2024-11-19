namespace Seminar.APPLICATION.Dtos.StatisticDtos;

public class StatisticsFilterDto
{
    public int? CompetitionId { get; set; }                 // Lọc theo cuộc thi (để nullable để có thể lọc tất cả)
    public int? Year { get; set; }                         // Lọc theo năm
    public int? DisciplineId { get; set; }                 // Lọc theo lĩnh vực
}