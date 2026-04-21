using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
