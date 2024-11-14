using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.CompetitionDtos;

public class UpdateDateEndCompetitionDto
{
    [Range(1, 12, ErrorMessage = "Month is invalid!")]
    public int Month { get; set; }
}
