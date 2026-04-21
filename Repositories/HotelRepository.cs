using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBookingAPI.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<HotelRepository> _logger;

    public HotelRepository(AppDbContext db, ILogger<HotelRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ── IBaseRepository<Hotel> ──────────────────────────────────
    public async Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return await _db.Hotels
            .Include(h => h.Rooms).ThenInclude(r => r.RoomCategory)
            .Include(h => h.Amenities)
            .Where(h => h.IsActive)
            .ToListAsync();
    }

    public async Task<Hotel?> GetByIdAsync(int id)
    {
        return await _db.Hotels
            .Include(h => h.Rooms).ThenInclude(r => r.RoomCategory)
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hotel> CreateAsync(Hotel entity)
    {
        _db.Hotels.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Hotel created: {HotelId} - {HotelName}", entity.Id, entity.Name);
        return entity;
    }

    public async Task<Hotel> UpdateAsync(Hotel entity)
    {
        _db.Hotels.Update(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Hotel updated: {HotelId}", entity.Id);
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var hotel = await _db.Hotels.FindAsync(id)
            ?? throw new KeyNotFoundException($"Hotel {id} not found.");
        hotel.IsActive = false;  // soft delete
        await _db.SaveChangesAsync();
        _logger.LogInformation("Hotel soft-deleted: {HotelId}", id);
    }

    public async Task<bool> ExistsAsync(int id)
        => await _db.Hotels.AnyAsync(h => h.Id == id);

    // ── IHotelRepository ───────────────────────────────────────
    public async Task<IEnumerable<Hotel>> SearchAsync(SearchFilterDto filter)
    {
        var query = _db.Hotels
            .Include(h => h.Rooms).ThenInclude(r => r.RoomCategory)
            .Include(h => h.Rooms).ThenInclude(r => r.Bookings)
            .Include(h => h.Amenities)
            .Where(h => h.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(h => h.Location.Contains(filter.Location));

        if (filter.Amenities != null && filter.Amenities.Any())
            query = query.Where(h => h.Amenities
                .Any(a => filter.Amenities.Contains(a.Name)));

        if (filter.MinPrice.HasValue)
            query = query.Where(h => h.Rooms
                .Any(r => r.PricePerNight >= filter.MinPrice.Value));

        if (filter.MaxPrice.HasValue)
            query = query.Where(h => h.Rooms
                .Any(r => r.PricePerNight <= filter.MaxPrice.Value));

        if (filter.Guests.HasValue)
            query = query.Where(h => h.Rooms
                .Any(r => r.MaxOccupancy >= filter.Guests.Value));

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Hotel>> GetByLocationAsync(string location)
    {
        return await _db.Hotels
            .Include(h => h.Rooms).ThenInclude(r => r.RoomCategory)
            .Include(h => h.Amenities)
            .Where(h => h.IsActive && h.Location.Contains(location))
            .ToListAsync();
    }

    public async Task<Hotel?> GetWithRoomsAndAmenitiesAsync(int hotelId)
    {
        return await _db.Hotels
            .Include(h => h.Rooms).ThenInclude(r => r.RoomCategory)
            .Include(h => h.Rooms).ThenInclude(r => r.Bookings)
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == hotelId);
    }

    public async Task<IEnumerable<Amenity>> GetAmenitiesAsync()
        => await _db.Amenities.ToListAsync();
}

