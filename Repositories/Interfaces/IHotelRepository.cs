using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repositories.Interfaces
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        Task<IEnumerable<Hotel>> SearchAsync(SearchFilterDto filter);
        Task<IEnumerable<Hotel>> GetByLocationAsync(string location);
        Task<Hotel?> GetWithRoomsAndAmenitiesAsync(int hotelId);
        Task<IEnumerable<Amenity>> GetAmenitiesAsync();
    }
}
