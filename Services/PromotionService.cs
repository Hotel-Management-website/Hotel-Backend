
using HotelBookingAPI.DTOs.Promotion;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;

namespace HotelBookingAPI.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _repo;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(IPromotionRepository repo, ILogger<PromotionService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // ── GetActivePromotionsAsync ──────────────────────────────────────

        public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync()
        {
            _logger.LogInformation("Fetching active promotions.");
            var list = await _repo.GetActivePromotionsAsync();
            return list.Select(ToDto);
        }

        // ── CreateAsync ───────────────────────────────────────────────────

        public async Task<PromotionDto> CreateAsync(PromotionDto dto)
        {
            _logger.LogInformation("Creating promotion: {Title}", dto.Title);

            var entity = new Promotion
            {
                Title = dto.Title,
                Description = dto.Description,
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };

            var saved = await _repo.CreateAsync(entity);
            _logger.LogInformation("Promotion {Id} created.", saved.Id);
            return ToDto(saved);
        }

        // ── ApplyDiscountCodeAsync — returns discounted price (decimal) ───
        //
        // Steps per spec:
        //  1. Find DiscountCode by Code string
        //  2. Validate: IsActive, ExpiryDate > now, UsedCount < MaxUses
        //  3. Compute discountedPrice = originalPrice * (1 - DiscountPercentage/100)
        //  4. IncrementCodeUsageAsync

        public async Task<decimal> ApplyDiscountCodeAsync(DiscountApplyDto dto)
        {
            _logger.LogInformation("Applying code '{Code}' to price {Price}.", dto.Code, dto.BookingTotalPrice);

            // 1. Look up
            var code = await _repo.GetDiscountCodeAsync(dto.Code);
            if (code is null)
            {
                _logger.LogWarning("Discount code '{Code}' not found.", dto.Code);
                throw new KeyNotFoundException($"Discount code '{dto.Code}' was not found.");
            }

            // 2. Validate
            if (!code.IsActive)
                throw new InvalidOperationException($"Discount code '{dto.Code}' is inactive.");

            if (code.ExpiryDate <= DateTime.UtcNow)
                throw new InvalidOperationException($"Discount code '{dto.Code}' has expired.");

            if (code.UsedCount >= code.MaxUses)
                throw new InvalidOperationException($"Discount code '{dto.Code}' has reached its usage limit.");

            // 3. Calculate
            decimal discounted = Math.Round(
                dto.BookingTotalPrice * (1m - code.DiscountPercentage / 100m), 2);

            // 4. Increment usage
            await _repo.IncrementCodeUsageAsync(code.Id);

            _logger.LogInformation(
                "Code '{Code}' applied. Original={Original}, Discounted={Discounted}.",
                dto.Code, dto.BookingTotalPrice, discounted);

            return discounted;
        }

        // ── ExpireAsync ───────────────────────────────────────────────────

        public async Task ExpireAsync(int promotionId)
        {
            _logger.LogInformation("Expiring promotion {Id}.", promotionId);

            if (!await _repo.ExistsAsync(promotionId))
                throw new KeyNotFoundException($"Promotion {promotionId} not found.");

            await _repo.ExpirePromotionAsync(promotionId);
            _logger.LogInformation("Promotion {Id} expired.", promotionId);
        }

        // ── Mapper ────────────────────────────────────────────────────────

        private static PromotionDto ToDto(Promotion p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            DiscountPercentage = p.DiscountPercentage,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsActive = p.IsActive
        };
    }
}