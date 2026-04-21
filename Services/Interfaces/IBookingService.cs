using HotelBooking.API.DTOs.Booking;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(BookingRequestDto dto, int userId);

        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId);

        Task<BookingDto?> GetByIdAsync(int bookingId);

        Task CancelBookingAsync(int bookingId, int userId);

        Task UpdateStatusAsync(int bookingId, BookingStatus status);
    }
}