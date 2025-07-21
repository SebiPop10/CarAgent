using CarAgent_BE.Models;
using CarAgent_BE.Contracts.Auth;

namespace CarAgent_BE.Services
{
    public interface ITokenService
    {
        TokenResponse GenerateTokenPair(UserLite user, IEnumerable<string> roles);
        bool ValidateAccessToken(string token); // opțional
        // Pentru refresh tokens vom avea un store mai târziu (DB). Deocamdată in-memory?
    }
}