using HotelBookingAPI.DTOs.Auth;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var tokens = await _authService.RegisterAsync(dto);
            return Ok(ApiHelper.Success(tokens, "Registration successful."));
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var tokens = await _authService.LoginAsync(dto);
            return Ok(ApiHelper.Success(tokens, "Login successful."));
        }

        // POST /api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var tokens = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(ApiHelper.Success(tokens, "Token refreshed."));
        }

        // POST /api/auth/revoke
        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken);
            return Ok(ApiHelper.Success<object>(null, "Token revoked."));
        }
    }
}
