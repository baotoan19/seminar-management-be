using AutoMapper;
using System.Security.Cryptography;
using Seminar.APPLICATION.Dtos.OtpDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.DOMAIN.Interfaces;
using Seminar.DOMAIN.Enum;
using Microsoft.EntityFrameworkCore;
using Seminar.CORE.ExceptionCustom;
using Microsoft.AspNetCore.Http;
using Seminar.CORE.Constants;

namespace Seminar.APPLICATION.Services;

public class OtpService : IOtpService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public OtpService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<string> GenerateOtp(OtpRequestDto otpRequestDto)
    {
        string otpCode = GenerateRandomOtpCode();
        OtpVerification otpVerification = new OtpVerification
        {
            Email = otpRequestDto.Email,
            OtpCode = otpCode,
            ExpiredTime = DateTime.Now.AddMinutes(2),
            IsUsed = false,
            AttemptCount = 0,
            OtpType = otpRequestDto.OtpType,
            OtpStatus = OtpStatusEnum.Pending
        };
        await _unitOfWork.GetRepository<OtpVerification>().InsertAsync(otpVerification);
        await _unitOfWork.SaveChangesAsync();
        await _emailService.SendOtpEmail(otpRequestDto.Email, otpCode);
        return otpCode;
    }

    public async Task<bool> VerifyOtp(OtpVerificationDto otpVerificationDto)
    {
        try
        {
            // Tìm OTP trong database dựa vào email (không check OTP code ngay)
            var otpVerification = await _unitOfWork.GetRepository<OtpVerification>()
                .Entities
                .FirstOrDefaultAsync(x => x.Email == otpVerificationDto.Email)
                ?? throw new ErrorException(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND, "No OTP request found for this email!");

            // Kiểm tra OTP đã được sử dụng chưa
            if (otpVerification.IsUsed && otpVerification.OtpStatus == OtpStatusEnum.Verified)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "OTP has already been used!");
            }

            // Kiểm tra OTP đã hết hạn chưa
            if (otpVerification.OtpStatus == OtpStatusEnum.Pending && otpVerification.ExpiredTime < DateTime.Now)
            {
                otpVerification.OtpStatus = OtpStatusEnum.Expired;
                await _unitOfWork.SaveChangesAsync();
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "OTP has expired!");
            }

            // Kiểm tra số lần thử
            if (otpVerification.AttemptCount >= 5)
            {
                otpVerification.OtpStatus = OtpStatusEnum.Cancelled;
                await _unitOfWork.SaveChangesAsync();
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Exceeded maximum number of attempts (5)!");
                
            }

            // Tăng số lần thử trước khi kiểm tra OTP
            otpVerification.AttemptCount++;
            await _unitOfWork.SaveChangesAsync();

            // Kiểm tra mã OTP có đúng không
            if (otpVerification.OtpCode != otpVerificationDto.OtpCode)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Invalid OTP code!");
            }

            // Nếu OTP đúng, đánh dấu đã sử dụng
            otpVerification.IsUsed = true;
            otpVerification.OtpStatus = OtpStatusEnum.Verified;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (ErrorException)
        {
            throw;
        }
    }

    public async Task<bool> ResendOtp(OtpRequestDto otpRequestDto)
    {
        try
        {
            // Kiểm tra OTP gần nhất của email này
            var lastOtp = await _unitOfWork.GetRepository<OtpVerification>()
                .Entities
                .Where(x => x.Email == otpRequestDto.Email &&
                x.OtpType == otpRequestDto.OtpType)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastOtp != null)
            {
                // Kiểm tra thời gian giữa 2 lần gửi OTP (2 phút)
                if (lastOtp.OtpStatus == OtpStatusEnum.Pending && DateTime.Now < lastOtp.ExpiredTime.AddMinutes(2))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest,
                        ResponseCodeConstants.BADREQUEST,
                        "Please wait 2 minutes before requesting a new OTP!");
                }

                // Vô hiệu hóa OTP cũ
                lastOtp.OtpStatus = OtpStatusEnum.Cancelled;
                lastOtp.IsUsed = true;
            }

            // Tạo mã OTP mới
            string otpCode = GenerateRandomOtpCode();
            OtpVerification newOtp = new OtpVerification
            {
                Email = otpRequestDto.Email,
                OtpCode = otpCode,
                ExpiredTime = DateTime.Now.AddMinutes(2),
                IsUsed = false,
                AttemptCount = 0,
                OtpType = otpRequestDto.OtpType,
                OtpStatus = OtpStatusEnum.Pending
            };

            await _unitOfWork.GetRepository<OtpVerification>().InsertAsync(newOtp);
            await _unitOfWork.SaveChangesAsync();
            await _emailService.SendOtpEmail(otpRequestDto.Email, otpCode);
            return true;
        }
        catch (ErrorException)
        {
            throw;
        }
    }

    private static string GenerateRandomOtpCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
    }



}