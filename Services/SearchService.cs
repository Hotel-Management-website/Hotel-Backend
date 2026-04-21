using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace HotelBookingAPI.Services;

public class SearchService : ISearchService
{
    private readonly IHotelRepository _hotelRepo;
    private readonly ILogger<SearchService> _logger;

    public SearchService(IHotelRepository hotelRepo, ILogger<SearchService> logger)
    {
        _hotelRepo = hotelRepo;
        _logger = logger;
    }

    public async Task<IEnumerable<HotelDto>> SearchHotelsAsync(SearchFilterDto filter)
    {
        _logger.LogInformation(
            "SearchService: Searching hotels — Location={Loc}, CheckIn={CI}, CheckOut={CO}",
            filter.Location, filter.CheckIn, filter.CheckOut);

        var hotels = await _hotelRepo.SearchAsync(filter);

        // If dates are provided, filter rooms by availability
        if (filter.CheckIn.HasValue && filter.CheckOut.HasValue)
        {
            var ci = filter.CheckIn.Value;
            var co = filter.CheckOut.Value;

            hotels = hotels.Where(h => h.Rooms.Any(r =>
                !r.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled &&
                    b.CheckInDate < co &&
                    b.CheckOutDate > ci)));
        }

        return hotels.Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Location = h.Location,
            Description = h.Description,
            ImageUrl = h.ImageUrl,
            StarRating = h.StarRating,
            IsActive = h.IsActive,
            Amenities = h.Amenities?.Select(a => a.Name).ToList() ?? new(),
            Rooms = h.Rooms?.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                PricePerNight = r.PricePerNight,
                MaxOccupancy = r.MaxOccupancy,
                IsAvailable = r.IsAvailable,
                ImageUrl = r.ImageUrl,
                HotelId = r.HotelId,
                CategoryName = r.RoomCategory?.Name ?? string.Empty
            }).ToList() ?? new()
        });
    }
}
