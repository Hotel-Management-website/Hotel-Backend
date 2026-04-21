using HotelBookingAPI.Data;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly AppDbContext _context;

    public AvailabilityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return !await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Status != HotelBookingAPI.Models.BookingStatus.Cancelled &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn
        );
    }

    public async Task UpdateAvailabilityOnBookingAsync(int roomId, bool isAvailable)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room != null)
        {
            room.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
        }
    }
}