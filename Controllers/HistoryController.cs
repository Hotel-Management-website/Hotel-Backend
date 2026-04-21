using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
