namespace HotelBookingAPI.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
