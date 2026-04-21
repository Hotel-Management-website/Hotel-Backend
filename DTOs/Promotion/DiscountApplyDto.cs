namespace HotelBookingAPI.DTOs.Promotion
{
    public class DiscountApplyDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal BookingTotalPrice { get; set; }
    }
}