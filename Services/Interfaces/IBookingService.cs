using HotelBookingAPI.DTOs.Booking;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ConfirmationDto> CreateBookingAsync(BookingRequestDto dto, int userId);
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId);
        Task<BookingDto?> GetByIdAsync(int bookingId);
        Task CancelBookingAsync(int bookingId, int userId);
        Task UpdateStatusAsync(int bookingId, BookingStatus status);  // Admin
    }
}
