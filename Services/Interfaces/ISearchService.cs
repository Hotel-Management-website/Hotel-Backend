using HotelBookingAPI.DTOs.Hotel;

namespace HotelBookingAPI.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<HotelDto>> SearchHotelsAsync(SearchFilterDto filter);
    }
}
