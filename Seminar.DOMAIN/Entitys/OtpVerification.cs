using System.ComponentModel.DataAnnotations.Schema;
using Seminar.CORE.Base;
using Seminar.DOMAIN.Enum;

[Table("OtpVerifications")]
public class OtpVerification : BaseEntity
{
    public string Email { get; set; }
    public string OtpCode { get; set; }
    public DateTime ExpiredTime { get; set; }
    public bool IsUsed { get; set; }
    public int AttemptCount { get; set; }
    public OtpTypeEnum OtpType { get; set; }
    public OtpStatusEnum OtpStatus { get; set; }
}