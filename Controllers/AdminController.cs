using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
