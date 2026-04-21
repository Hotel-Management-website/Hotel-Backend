using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<RoomRepository> _logger;

    public RoomRepository(AppDbContext db, ILogger<RoomRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ── IBaseRepository<Room> ──────────────────────────────────
    public async Task<IEnumerable<Room>> GetAllAsync()
        => await _db.Rooms
            .Include(r => r.RoomCategory)
            .Include(r => r.Hotel)
            .ToListAsync();

    public async Task<Room?> GetByIdAsync(int id)
        => await _db.Rooms
            .Include(r => r.RoomCategory)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Room> CreateAsync(Room entity)
    {
        _db.Rooms.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Room created: {RoomId} in hotel {HotelId}", entity.Id, entity.HotelId);
        return entity;
    }

    public async Task<Room> UpdateAsync(Room entity)
    {
        _db.Rooms.Update(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Room updated: {RoomId}", entity.Id);
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _db.Rooms.FindAsync(id)
            ?? throw new KeyNotFoundException($"Room {id} not found.");
        _db.Rooms.Remove(room);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Room deleted: {RoomId}", id);
    }

    public async Task<bool> ExistsAsync(int id)
        => await _db.Rooms.AnyAsync(r => r.Id == id);

    // ── IRoomRepository ────────────────────────────────────────
    public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId)
        => await _db.Rooms
            .Include(r => r.RoomCategory)
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut)
    {
        // A room is available when NO overlapping non-cancelled booking exists
        return await _db.Rooms
            .Include(r => r.RoomCategory)
            .Include(r => r.Bookings)
            .Where(r => r.HotelId == hotelId &&
                !r.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled &&
                    b.CheckInDate < checkOut &&
                    b.CheckOutDate > checkIn))
            .ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(
        int roomId, DateTime checkIn, DateTime checkOut)
    {
        return !await _db.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Status != BookingStatus.Cancelled &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn);
    }
}
