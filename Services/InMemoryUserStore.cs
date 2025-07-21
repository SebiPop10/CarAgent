using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using CarAgent_BE.Models;

namespace CarAgent_BE.Services
{
    public class InMemoryUserStore : IUserStoreLite
    {
        private readonly ConcurrentDictionary<string, UserLite> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);

        public Task<UserLite?> FindByEmailAsync(string email)
        {
            _usersByEmail.TryGetValue(email, out var user);
            return Task.FromResult(user);
        }

        public async Task<UserLite> CreateAsync(UserLite user, string passwordPlain)
        {
            user.PasswordHash = Hash(passwordPlain);
            _usersByEmail[user.Email] = user;
            return await Task.FromResult(user);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string passwordPlain)
        {
            var user = await FindByEmailAsync(email);
            if (user is null) return false;
            return Verify(user.PasswordHash, passwordPlain);
        }

        public Task<IReadOnlyList<string>> GetRolesAsync(UserLite user)
            => Task.FromResult((IReadOnlyList<string>)user.Roles);

        // -------- simplist: nu pentru producție --------
        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
        private static bool Verify(string hash, string input) => hash == Hash(input);
    }
}
