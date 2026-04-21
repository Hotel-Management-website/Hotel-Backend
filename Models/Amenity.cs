namespace HotelBookingAPI.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  // "WiFi", "Pool", "Gym"

        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
