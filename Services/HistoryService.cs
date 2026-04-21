
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Promotion;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;

namespace HotelBookingAPI.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepo;
        private readonly AppDbContext _context;
        private readonly ILogger<HistoryService> _logger;

        public HistoryService(
            IHistoryRepository historyRepo,
            AppDbContext context,
            ILogger<HistoryService> logger)
        {
            _historyRepo = historyRepo;
            _context = context;
            _logger = logger;
        }

        // ── GetUserHistoryAsync ───────────────────────────────────────────
        //
        // LoyaltyReward has no BookingId, so we cannot join rewards per booking.
        // We show EarnedPoints = 0 per booking row; the total points balance is
        // available via GET /api/loyalty/points.  If you later add a BookingId
        // column to LoyaltyReward the mapping below can be updated.

        public async Task<IEnumerable<HistoryDto>> GetUserHistoryAsync(int userId)
        {
            _logger.LogInformation("GetUserHistoryAsync: userId={UserId}.", userId);

            var bookings = await _historyRepo.GetUserHistoryAsync(userId);

            return bookings.Select(b => new HistoryDto
            {
                BookingId = b.Id,
                ReservationNumber = b.ReservationNumber,
                HotelName = b.Room?.Hotel?.Name ?? string.Empty,
                RoomNumber = b.Room?.RoomNumber ?? string.Empty,
                CheckInDate = b.CheckInDate,         // ← correct field name
                CheckOutDate = b.CheckOutDate,        // ← correct field name
                TotalPrice = b.TotalPrice,
                Status = b.Status,              // BookingStatus enum
                EarnedPoints = 0                      // no BookingId on LoyaltyReward
            });
        }

        // ── QuickRebookAsync ──────────────────────────────────────────────
        // Interface: Task<RebookDto> QuickRebookAsync(int bookingId, int userId)
        //
        // The controller reads NewCheckIn/NewCheckOut from [FromBody] RebookDto,
        // sets dto.BookingId = route {id}, then calls QuickRebookInternalAsync.
        // The interface-compliant method below is only used when dates are already
        // embedded in a RebookDto passed through the controller (see controller).
        //
        // Because the interface signature has no date parameters we expose the
        // two-arg version from the interface and a richer internal version that
        // the controller calls directly on the concrete service.

        /// <summary>
        /// Interface implementation — not directly callable without dates.
        /// The controller calls QuickRebookAsync(RebookDto, userId) below.
        /// </summary>
        public Task<RebookDto> QuickRebookAsync(int bookingId, int userId)
            => throw new NotSupportedException(
                "Call the overload QuickRebookAsync(RebookDto dto, int userId) from the controller.");

        /// <summary>
        /// Controller-facing overload. dto.BookingId is the original booking id.
        /// Populates dto.BookingId with the new booking id on return.
        /// </summary>
        public async Task<RebookDto> QuickRebookAsync(RebookDto dto, int userId)
        {
            _logger.LogInformation(
                "QuickRebookAsync: originalBookingId={Id}, userId={UserId}, {In}–{Out}.",
                dto.BookingId, userId, dto.NewCheckIn, dto.NewCheckOut);

            // 1. Fetch original booking
            var original = await _context.Bookings.FindAsync(dto.BookingId)
                ?? throw new KeyNotFoundException($"Booking {dto.BookingId} not found.");

            // 2. Validate new dates
            if (dto.NewCheckIn.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("NewCheckIn cannot be in the past.");

            if (dto.NewCheckIn >= dto.NewCheckOut)
                throw new ArgumentException("NewCheckIn must be before NewCheckOut.");

            // 3. Pro-rate price using original nightly rate
            int originalNights = Math.Max(1, (original.CheckOutDate - original.CheckInDate).Days);
            int newNights = Math.Max(1, (dto.NewCheckOut - dto.NewCheckIn).Days);
            decimal nightlyRate = original.TotalPrice / originalNights;

            // 4. Create new booking — same room, same user, new dates
            var newBooking = new Booking
            {
                UserId = userId,
                RoomId = original.RoomId,
                CheckInDate = dto.NewCheckIn,
                CheckOutDate = dto.NewCheckOut,
                NumberOfGuests = original.NumberOfGuests,
                TotalPrice = Math.Round(nightlyRate * newNights, 2),
                Status = BookingStatus.Pending,
                ReservationNumber = GenerateReservationNumber(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "QuickRebook created booking {NewId} for user {UserId}.", newBooking.Id, userId);

            // 5. Return RebookDto with the new booking id
            dto.BookingId = newBooking.Id;
            return dto;
        }

        // ── Helper ────────────────────────────────────────────────────────

        private static string GenerateReservationNumber()
            => $"RES-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}