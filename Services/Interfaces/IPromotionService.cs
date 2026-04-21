using HotelBookingAPI.DTOs.Promotion;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync();
        Task<PromotionDto> CreateAsync(PromotionDto dto);
        Task<decimal> ApplyDiscountCodeAsync(DiscountApplyDto dto);
        Task ExpireAsync(int promotionId);
    }
}
