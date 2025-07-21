using CarAgent_BE.Models;

namespace CarAgent_BE.Services
{
    public interface IUserStoreLite
    {
        Task<UserLite?> FindByEmailAsync(string email);
        Task<UserLite> CreateAsync(UserLite user, string passwordPlain);
        Task<bool> ValidateCredentialsAsync(string email, string passwordPlain);
        Task<IReadOnlyList<string>> GetRolesAsync(UserLite user);
    }
}
