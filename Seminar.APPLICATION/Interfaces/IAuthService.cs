using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;

namespace Seminar.APPLICATION.Interfaces{
    public interface IAuthService{
        Task SendRegistrationOtpAsync(string email);
        Task RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<TokenResponseDto> RefreshAccessTokenAsync(RefeshTokenRequestDto refeshTokenRequest);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
}