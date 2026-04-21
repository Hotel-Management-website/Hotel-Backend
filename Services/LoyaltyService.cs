
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;

namespace HotelBookingAPI.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly IHistoryRepository _historyRepo;
        private readonly AppDbContext _context;
        private readonly ILogger<LoyaltyService> _logger;

        public LoyaltyService(
            IHistoryRepository historyRepo,
            AppDbContext context,
            ILogger<LoyaltyService> logger)
        {
            _historyRepo = historyRepo;
            _context = context;
            _logger = logger;
        }

        // ── AddPointsAsync ────────────────────────────────────────────────
        // Points = (int)(booking.TotalPrice / 10)  → 1 point per ₹10 spent
        // LoyaltyReward fields: UserId, Points, RewardType, IsRedeemed, EarnedAt
        // (no BookingId on the model)

        public async Task AddPointsAsync(int userId, int bookingId)
        {
            _logger.LogInformation(
                "AddPointsAsync: userId={UserId}, bookingId={BookingId}.", userId, bookingId);

            var booking = await _context.Bookings.FindAsync(bookingId)
                ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

            int points = (int)(booking.TotalPrice / 10m);

            var reward = new LoyaltyReward
            {
                UserId = userId,
                Points = points,
                RewardType = "Earned",
                IsRedeemed = false,
                EarnedAt = DateTime.UtcNow
            };

            await _historyRepo.AddLoyaltyRewardAsync(reward);

            _logger.LogInformation(
                "Added {Points} loyalty points for user {UserId} (booking {BookingId}).",
                points, userId, bookingId);
        }

        // ── GetPointsAsync ────────────────────────────────────────────────
        // Sum all non-redeemed Points rows for this user

        public async Task<int> GetPointsAsync(int userId)
        {
            _logger.LogInformation("GetPointsAsync: userId={UserId}.", userId);
            return await _historyRepo.GetUserLoyaltyPointsAsync(userId);
        }

        // ── RedeemRewardAsync ─────────────────────────────────────────────
        // Deduct all available points by adding a negative-points redeemed row.
        // GetUserLoyaltyPointsAsync filters IsRedeemed == false, so a separate
        // negative row with IsRedeemed = true keeps the balance correct.

        public async Task RedeemRewardAsync(int userId, string rewardType)
        {
            _logger.LogInformation(
                "RedeemRewardAsync: userId={UserId}, rewardType={RewardType}.", userId, rewardType);

            int available = await _historyRepo.GetUserLoyaltyPointsAsync(userId);

            if (available <= 0)
                throw new InvalidOperationException(
                    $"User {userId} has no redeemable loyalty points.");

            // Mark existing earned rows as redeemed
            var earnedRewards = await _historyRepo.GetUserLoyaltyRewardsAsync(userId);
            var toRedeem = earnedRewards.Where(r => !r.IsRedeemed).ToList();

            foreach (var r in toRedeem)
                r.IsRedeemed = true;

            // Add a redemption record that shows what was redeemed and for what
            var redemptionRecord = new LoyaltyReward
            {
                UserId = userId,
                Points = 0,           // zero — the points are tracked on the earned rows
                RewardType = rewardType,  // e.g. "FreeNight" | "Discount"
                IsRedeemed = true,
                EarnedAt = DateTime.UtcNow
            };

            await _historyRepo.AddLoyaltyRewardAsync(redemptionRecord);
            await _context.SaveChangesAsync();   // persist the IsRedeemed=true updates

            _logger.LogInformation(
                "Redeemed {Points} points for user {UserId} as '{RewardType}'.",
                available, userId, rewardType);
        }
    }
}