using HotelBookingAPI.DTOs.Auth;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto> RegisterAsync(RegisterDto dto);
        Task<TokenDto> LoginAsync(LoginDto dto);
        Task<TokenDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }
}
