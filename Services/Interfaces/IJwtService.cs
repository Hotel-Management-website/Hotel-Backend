using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        int GetUserIdFromToken(string token);
    }
}
