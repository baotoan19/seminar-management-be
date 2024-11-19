using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.OtpDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers
{
    [ApiController]    
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        public AuthController(IAuthService authService, IOtpService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            await _authService.RegisterAsync(registerRequestDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Đăng ký thành công",
                data: null));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            LoginResponseDto loginResponseDto = await _authService.LoginAsync(loginRequestDto);
            return Ok(new BaseResponse<LoginResponseDto>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Đăng nhập thành công",
                data: loginResponseDto));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefeshTokenRequestDto refeshTokenRequest)
        {
            TokenResponseDto tokenResponseDto = await _authService.RefreshAccessTokenAsync(refeshTokenRequest);
            return Ok(new BaseResponse<TokenResponseDto>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Cập nhật token thành công",
                data: tokenResponseDto));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            await _authService.ChangePasswordAsync(changePasswordDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Đổi mật khẩu thành công",
                data: null));
        }

        [HttpPost("send-registration-otp")]
        public async Task<IActionResult> SendRegistrationOtp(string email)
        {
            await _authService.SendRegistrationOtpAsync(email);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Gửi OTP đăng ký thành công",
                data: null));
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(OtpRequestDto otpRequestDto)
        {
            await _otpService.ResendOtp(otpRequestDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Gửi OTP lại thành công",
                data: null));
        }
    }
}