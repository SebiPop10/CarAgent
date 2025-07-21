using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CarAgent_BE.Contracts.Auth;
using CarAgent_BE.Models;
using CarAgent_BE.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarAgent_BE.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly SymmetricSecurityKey _signingKey;

        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        }

        public TokenResponse GenerateTokenPair(UserLite user, IEnumerable<string> roles)
        {
            var now = DateTime.UtcNow;
            var accessExpires = now.AddMinutes(_options.AccessTokenMinutes);
            var refreshExpires = now.AddDays(_options.RefreshTokenDays);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("fullName", user.FullName ?? string.Empty)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: accessExpires,
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = GenerateSecureRandomToken();

            return new TokenResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresUtc = accessExpires,
                RefreshToken = refreshToken,
                RefreshTokenExpiresUtc = refreshExpires
            };
        }

        public bool ValidateAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _options.Issuer,
                    ValidAudience = _options.Audience,
                    IssuerSigningKey = _signingKey,
                    ClockSkew = TimeSpan.FromSeconds(30)
                }, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GenerateSecureRandomToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
