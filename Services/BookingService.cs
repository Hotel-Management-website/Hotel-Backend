using HotelBooking.API.DTOs.Booking;
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;

namespace HotelBooking.API.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repo;
    private readonly IAvailabilityService _availability;
    private readonly IEmailService _email;
    private readonly AppDbContext _context;

    public BookingService(
        IBookingRepository repo,
        IAvailabilityService availability,
        IEmailService email,
        AppDbContext context)
    {
        _repo = repo;
        _availability = availability;
        _email = email;
        _context = context;
    }

    public async Task<BookingDto> CreateBookingAsync(BookingRequestDto dto, int userId)
    {
        var room = await _context.Rooms.FindAsync(dto.RoomId)
            ?? throw new Exception("Room not found");

        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        var available = await _availability.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (!available)
            throw new Exception("Room not available");

        var days = (dto.CheckOutDate - dto.CheckInDate).Days;
        var total = room.PricePerNight * days;

        if (!string.IsNullOrEmpty(dto.DiscountCode))
        {
            var code = _context.DiscountCodes.FirstOrDefault(c => c.Code == dto.DiscountCode && c.IsActive);
            if (code != null)
            {
                total -= total * (code.DiscountPercentage / 100);
                code.UsedCount++;
            }
        }

        var booking = new Booking
        {
            ReservationNumber = $"HB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            RoomId = dto.RoomId,
            UserId = userId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            NumberOfGuests = dto.NumberOfGuests,
            TotalPrice = total
        };

        await _repo.CreateAsync(booking);

        await _availability.UpdateAvailabilityOnBookingAsync(dto.RoomId, false);

        var confirmation = new ConfirmationDto
        {
            ReservationNumber = booking.ReservationNumber,
            GuestName = user.FullName,
            HotelName = room.Hotel?.Name ?? "",
            RoomNumber = room.RoomNumber,
            CheckIn = booking.CheckInDate,
            CheckOut = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            EmailSentTo = user.Email
        };

        await _email.SendBookingConfirmationAsync(user.Email, confirmation);

        return new BookingDto
        {
            Id = booking.Id,
            ReservationNumber = booking.ReservationNumber,
            HotelName = confirmation.HotelName,
            RoomNumber = booking.Room.RoomNumber,
            CheckIn = booking.CheckInDate,
            CheckOut = booking.CheckOutDate,
            Guests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _repo.GetByUserIdAsync(userId);

        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            ReservationNumber = b.ReservationNumber,
            HotelName = b.Room.Hotel.Name,
            RoomNumber = b.Room.RoomNumber,
            CheckIn = b.CheckInDate,
            CheckOut = b.CheckOutDate,
            Guests = b.NumberOfGuests,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });
    }

    public async Task<BookingDto?> GetByIdAsync(int bookingId)
    {
        var b = await _repo.GetByIdAsync(bookingId);
        if (b == null) return null;

        return new BookingDto
        {
            Id = b.Id,
            ReservationNumber = b.ReservationNumber,
            HotelName = b.Room.Hotel.Name,
            RoomNumber = b.Room.RoomNumber,
            CheckIn = b.CheckInDate,
            CheckOut = b.CheckOutDate,
            Guests = b.NumberOfGuests,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        };
    }

    public async Task CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _repo.GetByIdAsync(bookingId)
            ?? throw new Exception("Booking not found");

        if (booking.UserId != userId)
            throw new Exception("Unauthorized");

        booking.Status = BookingStatus.Cancelled;
        await _repo.UpdateAsync(booking);

        await _availability.UpdateAvailabilityOnBookingAsync(booking.RoomId, true);

        await _email.SendCancellationEmailAsync(booking.User.Email, booking.ReservationNumber);
    }

    public async Task UpdateStatusAsync(int bookingId, BookingStatus status)
    {
        await _repo.UpdateStatusAsync(bookingId, status);
    }
}