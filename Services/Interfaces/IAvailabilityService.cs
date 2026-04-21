namespace HotelBookingAPI.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task UpdateAvailabilityOnBookingAsync(int roomId, bool isAvailable);
    }
}
