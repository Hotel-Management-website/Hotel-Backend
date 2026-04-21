using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/availability")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _service;

    public AvailabilityController(IAvailabilityService service)
    {
        _service = service;
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> Check(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var available = await _service.IsRoomAvailableAsync(roomId, checkIn, checkOut);
        return Ok(ApiHelper.Success(available));
    }
}