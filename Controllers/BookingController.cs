using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
