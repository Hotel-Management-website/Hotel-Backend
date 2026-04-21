using HotelBookingAPI.DTOs.Booking;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, ConfirmationDto confirmation);
        Task SendCancellationEmailAsync(string toEmail, string reservationNumber);
    }
}
