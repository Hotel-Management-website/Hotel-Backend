using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repositories.Interfaces
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetAllWithDetailsAsync();
        Task<Booking?> GetByReservationNumberAsync(string reservationNumber);
        Task UpdateStatusAsync(int bookingId, BookingStatus status);
    }
}
