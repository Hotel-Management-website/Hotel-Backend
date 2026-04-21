namespace HotelBookingAPI.DTOs.Hotel;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal StarRating { get; set; }
    public bool IsActive { get; set; } = true;
    public List<RoomDto> Rooms { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}

