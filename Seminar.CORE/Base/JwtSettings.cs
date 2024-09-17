namespace Seminar.CORE.Base
{
    public class JwtSettings
    {
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                throw new ArgumentException("SecretKey cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(Issuer))
            {
                throw new ArgumentException("Issuer cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(Audience))
            {
                throw new ArgumentException("Audience cannot be null or empty.");
            }

            if (AccessTokenExpirationMinutes <= 0)
            {
                throw new ArgumentException("AccessTokenExpirationMinutes must be greater than 0.");
            }

            if (RefreshTokenExpirationDays <= 0)
            {
                throw new ArgumentException("RefreshTokenExpirationDays must be greater than 0.");
            }

            return true;
        }
    }
}