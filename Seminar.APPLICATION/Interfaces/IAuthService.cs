using Microsoft.Identity.Client;
using Seminar.APPLICATION.Dtos.AuthDtos;

namespace Seminar.APPLICATION.Interfaces{
    public interface IAuthService{
        Task RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<TokenResponseDto> RefreshAccessTokenAsync(RefeshTokenRequestDto refeshTokenRequest);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
}