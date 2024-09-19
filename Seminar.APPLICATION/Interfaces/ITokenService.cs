using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces{
    public interface ITokenService{
        TokenResponseDto GenerateToken(Account account, string role);
        Task<TokenResponseDto> RefreshAccessToken(RefeshTokenRequestDto refeshTokenRequest);
    }
}