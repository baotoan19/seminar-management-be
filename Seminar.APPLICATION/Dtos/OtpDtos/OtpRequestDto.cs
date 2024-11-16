using Seminar.DOMAIN.Enum;

namespace Seminar.APPLICATION.Dtos.OtpDtos;

public class OtpRequestDto
{
    public string Email { get; set; }
    public OtpTypeEnum OtpType { get; set; }
}