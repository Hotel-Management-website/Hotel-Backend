using HotelBookingAPI.DTOs.Promotion;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryDto>> GetUserHistoryAsync(int userId);
        Task<RebookDto> QuickRebookAsync(int bookingId, int userId);
    }
}
