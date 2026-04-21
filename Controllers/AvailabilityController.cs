using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class AvailabilityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
