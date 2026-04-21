using HotelBookingAPI.DTOs.Promotion;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(IPromotionService service, ILogger<PromotionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET /api/promotions — public, no auth
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivePromotions()
        {
            var result = await _service.GetActivePromotionsAsync();
            // ApiHelper.Success returns IActionResult directly — do NOT wrap in Ok()
            return ApiHelper.Success(result, "Active promotions retrieved successfully.");
        }

        // POST /api/promotions — Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionDto dto)
        {
            if (!ModelState.IsValid)
                return ApiHelper.BadRequest("Invalid promotion data.");

            var created = await _service.CreateAsync(dto);
            return ApiHelper.Created(created, "Promotion created successfully.");
        }

        // POST /api/promotions/apply-code — authenticated users
        [HttpPost("apply-code")]
        [Authorize]
        public async Task<IActionResult> ApplyDiscountCode([FromBody] DiscountApplyDto dto)
        {
            if (!ModelState.IsValid)
                return ApiHelper.BadRequest("Invalid discount code request.");

            decimal discountedPrice = await _service.ApplyDiscountCodeAsync(dto);

            return ApiHelper.Success(
                new { OriginalPrice = dto.BookingTotalPrice, DiscountedPrice = discountedPrice },
                "Discount applied successfully.");
        }

        // PUT /api/promotions/{id}/expire — Admin only
        [HttpPut("{id:int}/expire")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExpirePromotion(int id)
        {
            await _service.ExpireAsync(id);
            return ApiHelper.Success<object?>(null, $"Promotion {id} expired successfully.");
        }
    }
}