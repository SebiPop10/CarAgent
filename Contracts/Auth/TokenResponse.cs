namespace CarAgent_BE.Contracts.Auth
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresUtc { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresUtc { get; set; }
    }
}
