using HotelBooking.API.DTOs.Booking;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(BookingRequestDto dto)
    {
        var result = await _service.CreateBookingAsync(dto, GetUserId());
        return Ok(ApiHelper.Success(result));
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var result = await _service.GetUserBookingsAsync(GetUserId());
        return Ok(ApiHelper.Success(result));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiHelper.Success(result));
    }

    [Authorize]
    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _service.CancelBookingAsync(id, GetUserId());
        return Ok(ApiHelper.Success("Cancelled"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] HotelBookingAPI.Models.BookingStatus status)
    {
        await _service.UpdateStatusAsync(id, status);
        return Ok(ApiHelper.Success("Updated"));
    }
}