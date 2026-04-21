namespace HotelBookingAPI.Models
{
    public class LoyaltyReward
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public string RewardType { get; set; } = string.Empty;  // "FreeNight", "Discount"
        public bool IsRedeemed { get; set; } = false;
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
