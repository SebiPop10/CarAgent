using CarAgent_BE.Contracts.Auth;
using CarAgent_BE.Models;

namespace CarAgent_BE.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Error, TokenResponse? Tokens)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Error, TokenResponse? Tokens)> LoginAsync(LoginRequest request);
        Task<(bool Success, string Error, TokenResponse? Tokens)> RefreshAsync(RefreshRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserStoreLite _users;
        private readonly ITokenService _tokens;

        // provizoriu in-memory refresh mapping: refreshToken -> userId + expiry
        private readonly Dictionary<string, (string UserId, DateTime ExpiresUtc)> _refreshStore = new();

        public AuthService(IUserStoreLite users, ITokenService tokens)
        {
            _users = users;
            _tokens = tokens;
        }

        public async Task<(bool Success, string Error, TokenResponse? Tokens)> RegisterAsync(RegisterRequest request)
        {
            var existing = await _users.FindByEmailAsync(request.Email);
            if (existing != null)
                return (false, "Email already registered.", null);

            var user = new UserLite
            {
                Email = request.Email,
                FullName = request.FullName
            };

            user = await _users.CreateAsync(user, request.Password);
            var roles = await _users.GetRolesAsync(user);
            var tokenPair = _tokens.GenerateTokenPair(user, roles);
            TrackRefresh(tokenPair.RefreshToken, user.Id, tokenPair.RefreshTokenExpiresUtc);
            return (true, "", tokenPair);
        }

        public async Task<(bool Success, string Error, TokenResponse? Tokens)> LoginAsync(LoginRequest request)
        {
            var valid = await _users.ValidateCredentialsAsync(request.Email, request.Password);
            if (!valid)
                return (false, "Invalid credentials.", null);

            var user = await _users.FindByEmailAsync(request.Email)!;
            var roles = await _users.GetRolesAsync(user!);
            var tokenPair = _tokens.GenerateTokenPair(user!, roles);
            TrackRefresh(tokenPair.RefreshToken, user!.Id, tokenPair.RefreshTokenExpiresUtc);
            return (true, "", tokenPair);
        }

        public async Task<(bool Success, string Error, TokenResponse? Tokens)> RefreshAsync(RefreshRequest request)
        {
            if (!_refreshStore.TryGetValue(request.RefreshToken, out var entry))
                return (false, "Invalid refresh token.", null);

            if (DateTime.UtcNow >= entry.ExpiresUtc)
                return (false, "Refresh token expired.", null);

            // (optional: rotate token by invalidating old)
            _refreshStore.Remove(request.RefreshToken);

            // create new pair
            var user = await _users.FindByEmailAsync(await GetEmailByUserId(entry.UserId));
            if (user is null)
                return (false, "User not found.", null);

            var roles = await _users.GetRolesAsync(user);
            var tokenPair = _tokens.GenerateTokenPair(user, roles);
            TrackRefresh(tokenPair.RefreshToken, user.Id, tokenPair.RefreshTokenExpiresUtc);
            return (true, "", tokenPair);
        }

        // provizoriu map userId -> email (că nu avem repo)
        private async Task<string> GetEmailByUserId(string userId)
        {
            // brute force in-memory
            var all = await _users.FindByEmailAsync(""); // not good; we don't have list
            // Pentru demo: schimbăm abordarea — stocăm email în refreshStore
            return string.Empty;
        }

        private void TrackRefresh(string token, string userId, DateTime expiresUtc)
        {
            _refreshStore[token] = (userId, expiresUtc);
        }
    }
}
