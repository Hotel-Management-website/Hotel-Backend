using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.API.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
        => await _context.Bookings.ToListAsync();

    public async Task<Booking?> GetByIdAsync(int id)
        => await _context.Bookings
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        => await _context.Bookings
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .Where(b => b.UserId == userId)
            .ToListAsync();

    public async Task<IEnumerable<Booking>> GetAllWithDetailsAsync()
        => await _context.Bookings
            .Include(b => b.Room).ThenInclude(r => r.Hotel)
            .Include(b => b.User)
            .ToListAsync();

    public async Task<Booking?> GetByReservationNumberAsync(string reservationNumber)
        => await _context.Bookings.FirstOrDefaultAsync(b => b.ReservationNumber == reservationNumber);

    public async Task<Booking> CreateAsync(Booking entity)
    {
        _context.Bookings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Booking> UpdateAsync(Booking entity)
    {
        _context.Bookings.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Bookings.AnyAsync(b => b.Id == id);

    public async Task UpdateStatusAsync(int bookingId, BookingStatus status)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            booking.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}