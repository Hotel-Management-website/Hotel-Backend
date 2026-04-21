namespace HotelBookingAPI.Models
{
    public class RoomCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  // e.g. "Deluxe", "Suite"
        public string Description { get; set; } = string.Empty;

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
