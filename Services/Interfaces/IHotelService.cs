using HotelBookingAPI.DTOs.Hotel;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetAllAsync();
        Task<HotelDto?> GetByIdAsync(int id);
        Task<HotelDto> CreateAsync(HotelDto dto);
        Task<HotelDto> UpdateAsync(int id, HotelDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<HotelDto>> SearchAsync(SearchFilterDto filter);
    }
}
