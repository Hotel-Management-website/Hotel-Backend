namespace HotelBookingAPI.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalHotels { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveBookings { get; set; }
    }
}
