using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<User> UpdateProfileAsync(int userId, string fullName);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
