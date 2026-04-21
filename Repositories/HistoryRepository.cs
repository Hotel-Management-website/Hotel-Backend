
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly AppDbContext _context;

        public HistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all bookings for a user, newest first,
        /// with Room → Hotel navigations loaded.
        /// </summary>
        public async Task<IEnumerable<Booking>> GetUserHistoryAsync(int userId)
            => await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        /// <summary>
        /// Returns all LoyaltyReward rows for the user.
        /// Note: LoyaltyReward has no BookingId — only UserId, Points, RewardType, IsRedeemed, EarnedAt.
        /// </summary>
        public async Task<IEnumerable<LoyaltyReward>> GetUserLoyaltyRewardsAsync(int userId)
            => await _context.LoyaltyRewards
                .Where(lr => lr.UserId == userId)
                .OrderByDescending(lr => lr.EarnedAt)
                .ToListAsync();

        /// <summary>
        /// Persists a new LoyaltyReward row.
        /// </summary>
        public async Task AddLoyaltyRewardAsync(LoyaltyReward reward)
        {
            _context.LoyaltyRewards.Add(reward);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sum of Points for all non-redeemed rewards belonging to this user.
        /// </summary>
        public async Task<int> GetUserLoyaltyPointsAsync(int userId)
            => await _context.LoyaltyRewards
                .Where(lr => lr.UserId == userId && !lr.IsRedeemed)
                .SumAsync(lr => lr.Points);
    }
}