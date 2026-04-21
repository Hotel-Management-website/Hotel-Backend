using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class PromotionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
