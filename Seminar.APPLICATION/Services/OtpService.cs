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
                .Where(x => x.Email == otpVerificationDto.Email)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND, "Không tìm thấy yêu cầu OTP cho email này. Vui lòng cung cấp email hợp lệ.");

            // Kiểm tra OTP đã được sử dụng chưa
            if (otpVerification.IsUsed && otpVerification.OtpStatus == OtpStatusEnum.Verified)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "OTP đã được sử dụng.");
            }

            // Kiểm tra OTP đã hết hạn chưa
            if (otpVerification.OtpStatus == OtpStatusEnum.Pending && otpVerification.ExpiredTime < DateTime.Now)
            {
                otpVerification.OtpStatus = OtpStatusEnum.Expired;
                await _unitOfWork.SaveChangesAsync();
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "OTP đã hết hạn.");
            }

            // Kiểm tra số lần thử
            if (otpVerification.AttemptCount >= 5)
            {
                otpVerification.OtpStatus = OtpStatusEnum.Cancelled;
                await _unitOfWork.SaveChangesAsync();
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Đã đạt số lần thử tối đa (5).");
                
            }

            // Tăng số lần thử trước khi kiểm tra OTP
            otpVerification.AttemptCount++;
            await _unitOfWork.SaveChangesAsync();

            // Kiểm tra mã OTP có đúng không
            if (otpVerification.OtpCode != otpVerificationDto.OtpCode)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Mã OTP không hợp lệ.");
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
                if (lastOtp.OtpStatus == OtpStatusEnum.Pending && DateTime.Now < lastOtp.CreatedAt.AddMinutes(2))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest,
                        ResponseCodeConstants.BADREQUEST,
                        "Vui lòng chờ 2 phút trước khi yêu cầu mã OTP mới.");
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