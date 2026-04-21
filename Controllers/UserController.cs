using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userService.GetByIdAsync(GetCurrentUserId());
            if (user == null)
                return NotFound(ApiHelper.NotFound("User not found."));

            return Ok(ApiHelper.Success(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.IsActive,
                user.CreatedAt
            }));
        }

        // PUT /api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            await _userService.UpdateProfileAsync(GetCurrentUserId(), request.FullName);
            return Ok(ApiHelper.Success(true, "Profile updated successfully."));
        }

        // PUT /api/user/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await _userService.ChangePasswordAsync(
                GetCurrentUserId(),
                request.CurrentPassword,
                request.NewPassword);

            return Ok(ApiHelper.Success<object>(null, "Password changed successfully."));
        }
    }

    public record UpdateProfileRequest(string FullName);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
