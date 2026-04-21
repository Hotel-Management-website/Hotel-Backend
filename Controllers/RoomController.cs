using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
