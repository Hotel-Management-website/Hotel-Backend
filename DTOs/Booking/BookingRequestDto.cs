namespace HotelBooking.API.DTOs.Booking;

public class BookingRequestDto
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string? DiscountCode { get; set; }
}