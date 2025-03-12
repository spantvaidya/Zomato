using Microsoft.AspNetCore.Mvc;

namespace Zomato.Web.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
