using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Admin;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace HotelBookingAPI.Controllers
{
    [ApiController]
[Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService,
            AppDbContext context,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _context = context;
            _logger = logger;
        }

        // GET /api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(ApiHelper.Success(stats));
        }

        // GET /api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(ApiHelper.Success(users));
        }

        // PUT /api/admin/users/{id}/toggle
        [HttpPut("users/{id:int}/toggle")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            await _adminService.ToggleUserStatusAsync(id);
            return Ok(ApiHelper.Success<object>(null, "User status toggled."));
        }

        // PUT /api/admin/users/{id}/role
        [HttpPut("users/{id:int}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] ChangeRoleDto dto)
        {
            await _adminService.ChangeUserRoleAsync(id, dto.Role);
            return Ok(ApiHelper.Success<object>(null, "User role updated."));
        }

        // GET /api/admin/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.Hotel)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    b.Status,
                    b.CheckInDate,
                    b.CheckOutDate,
                    b.TotalPrice,
                    b.CreatedAt,
                    User = new { b.User!.Id, b.User.FullName, b.User.Email },
                    Room = new { b.Room!.Id, b.Room.RoomNumber, Hotel = new { b.Room.Hotel!.Id, b.Room.Hotel.Name } }
                })
                .ToListAsync();

            return Ok(ApiHelper.Success(bookings));
        }

        // PUT /api/admin/bookings/{id}/status
        [HttpPut("bookings/{id:int}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound(ApiHelper.NotFound($"Booking {id} not found."));

            if (!Enum.TryParse<BookingStatus>(dto.Status, out var parsedStatus))
                return BadRequest(ApiHelper.BadRequest($"Invalid status: {dto.Status}"));

            booking.Status = parsedStatus;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} status changed to {Status}", id, dto.Status);
            return Ok(ApiHelper.Success<object>(null, "Booking status updated."));
        }
    }
}
