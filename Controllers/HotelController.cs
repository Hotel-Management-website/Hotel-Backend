using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ISearchService _searchService;
    private readonly ILogger<HotelController> _logger;

    public HotelController(
        IHotelService hotelService,
        ISearchService searchService,
        ILogger<HotelController> logger)
    {
        _hotelService = hotelService;
        _searchService = searchService;
        _logger = logger;
    }

    // GET /api/hotels  [public]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _hotelService.GetAllAsync();
        return ApiHelper.Success(hotels, "Hotels retrieved successfully.");
    }

    // GET /api/hotels/{id}  [public]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel == null) return ApiHelper.NotFound($"Hotel {id} not found.");
        return ApiHelper.Success(hotel);
    }

    // GET /api/hotels/search?Location=...&CheckIn=...  [public]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchFilterDto filter)
    {
        _logger.LogInformation("Hotel search — Location: {Location}", filter.Location);
        var results = await _searchService.SearchHotelsAsync(filter);
        return ApiHelper.Success(results, "Search completed.");
    }

    // POST /api/hotels  [Admin]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] HotelDto dto)
    {
        var created = await _hotelService.CreateAsync(dto);
        return ApiHelper.Created(created, "Hotel created successfully.");
    }

    // PUT /api/hotels/{id}  [Admin]
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] HotelDto dto)
    {
        var updated = await _hotelService.UpdateAsync(id, dto);
        return ApiHelper.Success(updated, "Hotel updated successfully.");
    }

    // DELETE /api/hotels/{id}  [Admin]
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);
        return ApiHelper.Success<object>(null!, "Hotel deleted successfully.");
    }
}
