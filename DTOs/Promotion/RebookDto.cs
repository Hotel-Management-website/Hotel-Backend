namespace HotelBookingAPI.DTOs.Promotion
{
    /// <summary>
    /// Dual-purpose DTO:
    ///   Request  → client sends NewCheckIn + NewCheckOut in the body.
    ///   Response → service populates BookingId of the newly created booking.
    /// Matches interface: Task&lt;RebookDto&gt; QuickRebookAsync(int bookingId, int userId)
    /// </summary>
    public class RebookDto
    {
        // ── Sent by client ────────────────────────────────────────────────
        public DateTime NewCheckIn { get; set; }
        public DateTime NewCheckOut { get; set; }

        // ── Populated by service and returned ────────────────────────────
        public int BookingId { get; set; }
    }
}