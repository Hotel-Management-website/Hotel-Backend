using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
