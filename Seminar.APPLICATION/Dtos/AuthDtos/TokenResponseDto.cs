namespace Seminar.APPLICATION.Dtos.AuthDtos
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string Expires { get; set; }
        public required ResponseAccountDto Account { get; set; }
    }
}