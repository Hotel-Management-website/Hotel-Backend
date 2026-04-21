using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Admin;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IUserRepository userRepository,
            AppDbContext context,
            ILogger<AdminService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalHotels = await _context.Hotels.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var totalRevenue = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0m;
            var activeBookings = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Confirmed);

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalHotels = totalHotels,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                ActiveBookings = activeBookings
            };
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(u => new AdminUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task ToggleUserStatusAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            await _userRepository.ToggleActiveStatusAsync(userId);
            _logger.LogInformation("Toggled active status for user ID: {UserId} → IsActive={Status}", userId, !user.IsActive);
        }

        public async Task ChangeUserRoleAsync(int userId, string role)
        {
            var validRoles = new[] { "User", "Admin", "Staff" };
            if (!validRoles.Contains(role))
                throw new ArgumentException($"Invalid role: {role}. Valid roles: {string.Join(", ", validRoles)}");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            await _userRepository.UpdateRoleAsync(userId, role);
            _logger.LogInformation("Role changed for user ID: {UserId} → Role={Role}", userId, role);
        }
    }
}
