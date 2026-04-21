using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repositories.Interfaces
{
    public interface IHistoryRepository
    {
        Task<IEnumerable<Booking>> GetUserHistoryAsync(int userId);
        Task<IEnumerable<LoyaltyReward>> GetUserLoyaltyRewardsAsync(int userId);
        Task AddLoyaltyRewardAsync(LoyaltyReward reward);
        Task<int> GetUserLoyaltyPointsAsync(int userId);
    }
}
