using HotelBookingAPI.Models;

namespace HotelBooking.API.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Guests { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}