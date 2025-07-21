using CarAgent_BE.Contracts.Auth;
using CarAgent_BE.Services;
//using CarAgent_BE.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CarAgent_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (success, error, tokens) = await _auth.RegisterAsync(request);
            if (!success)
                return BadRequest(new { error });
            return Ok(tokens);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, error, tokens) = await _auth.LoginAsync(request);
            if (!success)
                return Unauthorized(new { error });
            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var (success, error, tokens) = await _auth.RefreshAsync(request);
            if (!success)
                return BadRequest(new { error });
            return Ok(tokens);
        }
    }
}
