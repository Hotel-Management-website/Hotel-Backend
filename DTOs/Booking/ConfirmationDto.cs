namespace HotelBooking.API.DTOs.Booking;

public class ConfirmationDto
{
    public string ReservationNumber { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public string EmailSentTo { get; set; } = string.Empty;
}