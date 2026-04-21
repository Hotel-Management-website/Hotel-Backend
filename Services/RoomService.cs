using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HotelBookingAPI.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly ILogger<RoomService> _logger;

    public RoomService(IRoomRepository roomRepo, ILogger<RoomService> logger)
    {
        _roomRepo = roomRepo;
        _logger = logger;
    }

    public async Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId)
    {
        var rooms = await _roomRepo.GetByHotelIdAsync(hotelId);
        return rooms.Select(MapToDto);
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        return room == null ? null : MapToDto(room);
    }

    public async Task<RoomDto> CreateAsync(RoomDto dto)
    {
        var room = new Room
        {
            RoomNumber = dto.RoomNumber,
            PricePerNight = dto.PricePerNight,
            MaxOccupancy = dto.MaxOccupancy,
            IsAvailable = dto.IsAvailable,
            ImageUrl = dto.ImageUrl,
            HotelId = dto.HotelId,
            RoomCategoryId = dto.RoomCategoryId
        };
        var created = await _roomRepo.CreateAsync(room);
        _logger.LogInformation("RoomService: Created room {RoomId} in hotel {HotelId}",
            created.Id, created.HotelId);
        return MapToDto(created);
    }

    public async Task<RoomDto> UpdateAsync(int id, RoomDto dto)
    {
        var room = await _roomRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Room {id} not found.");

        room.RoomNumber = dto.RoomNumber;
        room.PricePerNight = dto.PricePerNight;
        room.MaxOccupancy = dto.MaxOccupancy;
        room.IsAvailable = dto.IsAvailable;
        room.ImageUrl = dto.ImageUrl;
        room.RoomCategoryId = dto.RoomCategoryId;

        var updated = await _roomRepo.UpdateAsync(room);
        _logger.LogInformation("RoomService: Updated room {RoomId}", id);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _roomRepo.ExistsAsync(id))
            throw new KeyNotFoundException($"Room {id} not found.");
        await _roomRepo.DeleteAsync(id);
        _logger.LogInformation("RoomService: Deleted room {RoomId}", id);
    }

    public async Task<IEnumerable<RoomAvailabilityDto>> GetAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut)
    {
        if (checkIn >= checkOut)
            throw new ArgumentException("CheckIn must be before CheckOut.");

        var rooms = await _roomRepo.GetAvailableRoomsAsync(hotelId, checkIn, checkOut);
        return rooms.Select(r => new RoomAvailabilityDto
        {
            RoomId = r.Id,
            RoomNumber = r.RoomNumber,
            CategoryName = r.RoomCategory?.Name ?? string.Empty,
            PricePerNight = r.PricePerNight,
            MaxOccupancy = r.MaxOccupancy,
            IsAvailable = true,
            ImageUrl = r.ImageUrl
        });
    }

    private static RoomDto MapToDto(Room r) => new()
    {
        Id = r.Id,
        RoomNumber = r.RoomNumber,
        PricePerNight = r.PricePerNight,
        MaxOccupancy = r.MaxOccupancy,
        IsAvailable = r.IsAvailable,
        ImageUrl = r.ImageUrl,
        HotelId = r.HotelId,
        HotelName = r.Hotel?.Name ?? string.Empty,
        RoomCategoryId = r.RoomCategoryId,
        CategoryName = r.RoomCategory?.Name ?? string.Empty
    };
}

