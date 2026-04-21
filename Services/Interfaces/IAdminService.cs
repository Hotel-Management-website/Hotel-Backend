using HotelBookingAPI.DTOs.Admin;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardStatsAsync();
        Task<IEnumerable<AdminUserDto>> GetAllUsersAsync();
        Task ToggleUserStatusAsync(int userId);
        Task ChangeUserRoleAsync(int userId, string role);
    }
}
