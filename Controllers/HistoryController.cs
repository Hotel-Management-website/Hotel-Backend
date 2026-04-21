using System.Security.Claims;
using HotelBookingAPI.DTOs.Promotion;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    // ══════════════════════════════════════════════════════════════════════
    // GET  /api/history/my
    // POST /api/history/rebook/{id}
    // ══════════════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/history")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        // Concrete service injected (not interface) so the overloaded
        // QuickRebookAsync(RebookDto, userId) is accessible.
        // Register both IHistoryService and HistoryService in Program.cs:
        //   builder.Services.AddScoped<IHistoryService, HistoryService>();
        //   builder.Services.AddScoped<HistoryService>();
        private readonly HistoryService _historyService;
        private readonly ILogger<HistoryController> _logger;

        public HistoryController(HistoryService historyService, ILogger<HistoryController> logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        // GET /api/history/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyHistory()
        {
            int userId = GetUserId();
            var history = await _historyService.GetUserHistoryAsync(userId);
            return ApiHelper.Success(history, "Booking history retrieved successfully.");
        }

        // POST /api/history/rebook/{id}
        // Body: { "newCheckIn": "2025-09-01", "newCheckOut": "2025-09-05" }
        [HttpPost("rebook/{id:int}")]
        public async Task<IActionResult> QuickRebook(int id, [FromBody] RebookDto dto)
        {
            if (!ModelState.IsValid)
                return ApiHelper.BadRequest("Invalid rebook request.");

            dto.BookingId = id;   // route id = original booking id
            int userId = GetUserId();

            var result = await _historyService.QuickRebookAsync(dto, userId);
            return ApiHelper.Success(result, "Booking re-created successfully.");
        }

        private int GetUserId()
            => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    // ══════════════════════════════════════════════════════════════════════
    // GET  /api/loyalty/points
    // POST /api/loyalty/redeem
    // ══════════════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/loyalty")]
    [Authorize]
    public class LoyaltyController : ControllerBase
    {
        private readonly ILoyaltyService _loyaltyService;
        private readonly ILogger<LoyaltyController> _logger;

        public LoyaltyController(ILoyaltyService loyaltyService, ILogger<LoyaltyController> logger)
        {
            _loyaltyService = loyaltyService;
            _logger = logger;
        }

        // GET /api/loyalty/points
        [HttpGet("points")]
        public async Task<IActionResult> GetPoints()
        {
            int userId = GetUserId();
            int points = await _loyaltyService.GetPointsAsync(userId);
            return ApiHelper.Success(new { Points = points }, "Loyalty points retrieved.");
        }

        // POST /api/loyalty/redeem
        // Body: { "rewardType": "FreeNight" }
        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemReward([FromBody] RedeemRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RewardType))
                return ApiHelper.BadRequest("RewardType is required.");

            int userId = GetUserId();
            await _loyaltyService.RedeemRewardAsync(userId, dto.RewardType);
            return ApiHelper.Success<object?>(null, "Reward redeemed successfully.");
        }

        private int GetUserId()
            => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    // Inline request DTO for the redeem endpoint
    public sealed class RedeemRequestDto
    {
        public string RewardType { get; set; } = string.Empty;
    }
}