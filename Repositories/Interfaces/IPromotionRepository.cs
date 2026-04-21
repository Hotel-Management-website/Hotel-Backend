using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repositories.Interfaces
{
    public interface IPromotionRepository : IBaseRepository<Promotion>
    {
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<DiscountCode?> GetDiscountCodeAsync(string code);
        Task IncrementCodeUsageAsync(int codeId);
        Task ExpirePromotionAsync(int promotionId);
    }
}
