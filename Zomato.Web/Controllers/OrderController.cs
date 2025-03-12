using Microsoft.AspNetCore.Mvc;

namespace Zomato.Web.Controllers
{
    public class OrderController : Controller
    {
        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
        }
    }
}
