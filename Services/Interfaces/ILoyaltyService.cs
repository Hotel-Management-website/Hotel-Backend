namespace HotelBookingAPI.Services.Interfaces
{
    public interface ILoyaltyService
    {
        Task AddPointsAsync(int userId, int bookingId);
        Task<int> GetPointsAsync(int userId);
        Task RedeemRewardAsync(int userId, string rewardType);
    }
}
