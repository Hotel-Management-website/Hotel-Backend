namespace HotelBookingAPI.DTOs.Hotel;

public class SearchFilterDto
{
    public string? Location { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Guests { get; set; }
    public List<string>? Amenities { get; set; }
}

