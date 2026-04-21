using HotelBookingAPI.Helpers;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/amenities")]
public class AmenityController : ControllerBase
{
    private readonly IHotelRepository _hotelRepo;
    private readonly ILogger<AmenityController> _logger;

    public AmenityController(IHotelRepository hotelRepo, ILogger<AmenityController> logger)
    {
        _hotelRepo = hotelRepo;
        _logger = logger;
    }

    // GET /api/amenities  [public]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("AmenityController: Fetching all amenities");
        var amenities = await _hotelRepo.GetAmenitiesAsync();
        var names = amenities.Select(a => new { a.Id, a.Name });
        return ApiHelper.Success(names, "Amenities retrieved.");
    }
}
