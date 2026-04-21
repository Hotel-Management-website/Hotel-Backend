
using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Helpers;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomController> _logger;

    public RoomController(IRoomService roomService, ILogger<RoomController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    // GET /api/rooms/hotel/{hotelId}  [public]
    [HttpGet("hotel/{hotelId:int}")]
    public async Task<IActionResult> GetByHotel(int hotelId)
    {
        var rooms = await _roomService.GetByHotelIdAsync(hotelId);
        return ApiHelper.Success(rooms, "Rooms retrieved.");
    }

    // GET /api/rooms/{id}  [public]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null) return ApiHelper.NotFound($"Room {id} not found.");
        return ApiHelper.Success(room);
    }

    // GET /api/rooms/available/{hotelId}?checkIn=...&checkOut=...  [public]
    [HttpGet("available/{hotelId:int}")]
    public async Task<IActionResult> GetAvailable(
        int hotelId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut)
    {
        _logger.LogInformation(
            "Availability check — Hotel: {HotelId}, CheckIn: {CI}, CheckOut: {CO}",
            hotelId, checkIn, checkOut);
        var rooms = await _roomService.GetAvailableRoomsAsync(hotelId, checkIn, checkOut);
        return ApiHelper.Success(rooms, "Available rooms fetched.");
    }

    // POST /api/rooms  [Admin]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] RoomDto dto)
    {
        var created = await _roomService.CreateAsync(dto);
        return ApiHelper.Created(created, "Room created successfully.");
    }

    // PUT /api/rooms/{id}  [Admin]
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] RoomDto dto)
    {
        var updated = await _roomService.UpdateAsync(id, dto);
        return ApiHelper.Success(updated, "Room updated successfully.");
    }

    // DELETE /api/rooms/{id}  [Admin]
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _roomService.DeleteAsync(id);
        return ApiHelper.Success<object>(null!, "Room deleted.");
    }
}
