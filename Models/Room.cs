namespace HotelBookingAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string ImageUrl { get; set; } = string.Empty;

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public int RoomCategoryId { get; set; }
        public RoomCategory RoomCategory { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
