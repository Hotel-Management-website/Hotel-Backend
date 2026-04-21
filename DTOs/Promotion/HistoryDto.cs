
using HotelBookingAPI.Models;

namespace HotelBookingAPI.DTOs.Promotion
{
    public class HistoryDto
    {
        public int BookingId { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public int EarnedPoints { get; set; }
    }
}