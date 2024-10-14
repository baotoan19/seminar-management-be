using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.AuthDtos;
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
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            await _authService.RegisterAsync(registerRequestDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Register success "));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            LoginResponseDto loginResponseDto = await _authService.LoginAsync(loginRequestDto);
            return Ok(new BaseResponse<LoginResponseDto>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: loginResponseDto));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefeshTokenRequestDto refeshTokenRequest)
        {
            TokenResponseDto tokenResponseDto = await _authService.RefreshAccessTokenAsync(refeshTokenRequest);
            return Ok(new BaseResponse<TokenResponseDto>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: tokenResponseDto));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            await _authService.ChangePasswordAsync(changePasswordDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Change password success"));
        }
    }
}