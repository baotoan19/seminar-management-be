using Seminar.APPLICATION.Dtos.OtpDtos;
using Seminar.DOMAIN.Enum;

namespace Seminar.APPLICATION.Interfaces;

public interface IOtpService
{
    Task<string> GenerateOtp(OtpRequestDto otpRequestDto);
    Task<bool> VerifyOtp(OtpVerificationDto otpVerificationDto);
    Task<bool> ResendOtp(OtpRequestDto otpRequestDto);
}