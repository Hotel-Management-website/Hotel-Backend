namespace HotelBookingAPI.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public int MaxUses { get; set; }
        public int UsedCount { get; set; } = 0;
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;

        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; } = null!;
    }
}
