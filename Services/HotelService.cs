using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HotelBookingAPI.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepo;
    private readonly ILogger<HotelService> _logger;

    public HotelService(IHotelRepository hotelRepo, ILogger<HotelService> logger)
    {
        _hotelRepo = hotelRepo;
        _logger = logger;
    }

    public async Task<IEnumerable<HotelDto>> GetAllAsync()
    {
        var hotels = await _hotelRepo.GetAllAsync();
        return hotels.Select(MapToDto);
    }

    public async Task<HotelDto?> GetByIdAsync(int id)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id);
        return hotel == null ? null : MapToDto(hotel);
    }

    public async Task<HotelDto> CreateAsync(HotelDto dto)
    {
        var hotel = new Hotel
        {
            Name = dto.Name,
            Location = dto.Location,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            StarRating = dto.StarRating,
            IsActive = true
        };
        var created = await _hotelRepo.CreateAsync(hotel);
        _logger.LogInformation("HotelService: Created hotel {HotelId}", created.Id);
        return MapToDto(created);
    }

    public async Task<HotelDto> UpdateAsync(int id, HotelDto dto)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Hotel {id} not found.");

        hotel.Name = dto.Name;
        hotel.Location = dto.Location;
        hotel.Description = dto.Description;
        hotel.ImageUrl = dto.ImageUrl;
        hotel.StarRating = dto.StarRating;
        hotel.IsActive = dto.IsActive;

        var updated = await _hotelRepo.UpdateAsync(hotel);
        _logger.LogInformation("HotelService: Updated hotel {HotelId}", id);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _hotelRepo.ExistsAsync(id))
            throw new KeyNotFoundException($"Hotel {id} not found.");
        await _hotelRepo.DeleteAsync(id);
        _logger.LogInformation("HotelService: Deleted hotel {HotelId}", id);
    }

    public async Task<IEnumerable<HotelDto>> SearchAsync(SearchFilterDto filter)
    {
        var hotels = await _hotelRepo.SearchAsync(filter);
        return hotels.Select(MapToDto);
    }

    // ── Mapper ─────────────────────────────────────────────────
    private static HotelDto MapToDto(Hotel h) => new()
    {
        Id = h.Id,
        Name = h.Name,
        Location = h.Location,
        Description = h.Description,
        ImageUrl = h.ImageUrl,
        StarRating = h.StarRating,
        IsActive = h.IsActive,
        Rooms = h.Rooms?.Select(r => new RoomDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            PricePerNight = r.PricePerNight,
            MaxOccupancy = r.MaxOccupancy,
            IsAvailable = r.IsAvailable,
            ImageUrl = r.ImageUrl,
            HotelId = r.HotelId,
            RoomCategoryId = r.RoomCategoryId,
            CategoryName = r.RoomCategory?.Name ?? string.Empty
        }).ToList() ?? new(),
        Amenities = h.Amenities?.Select(a => a.Name).ToList() ?? new()
    };
}

