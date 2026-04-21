using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task ToggleActiveStatusAsync(int userId);
        Task UpdateRoleAsync(int userId, string role);
    }
}
