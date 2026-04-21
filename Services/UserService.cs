using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;

namespace HotelBookingAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> UpdateProfileAsync(int userId, string fullName)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            user.FullName = fullName;
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Profile updated for user ID: {UserId}", userId);
            return user;
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password changed for user ID: {UserId}", userId);
        }
    }
}
