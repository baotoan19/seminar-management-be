using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AcceptanceDtos;

public class UpdateAcceptanceForPublicationDto
{
    [Range(1, 2, ErrorMessage = "AcceptedForPublicationStatus is invalid!")]
    public int AcceptedForPublicationStatus { get; set; }
}
