using HotelBookingAPI.DTOs.Hotel;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId);
        Task<RoomDto?> GetByIdAsync(int id);
        Task<RoomDto> CreateAsync(RoomDto dto);
        Task<RoomDto> UpdateAsync(int id, RoomDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<RoomAvailabilityDto>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut);
    }
}
