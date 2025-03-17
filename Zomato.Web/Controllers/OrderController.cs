using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Controllers
{
    public class OrderController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        public IActionResult OrderIndex(string? status)
        {
            return View();
        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string? status)
        {
            IEnumerable<OrderHeaderDto> orders;
            string userId = string.Empty;

            if (string.IsNullOrEmpty(userId)) userId = string.Empty;

            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!User.IsInRole(SD.RoleAdmin))
                userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto response = _orderService.GetAllOrders(userId).GetAwaiter().GetResult();
            if(response != null && response.IsSuccess)
            {
                orders = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDto>>(Convert.ToString(response.Result));
            }
            else
            {
                orders = new List<OrderHeaderDto>();
            }

            return Json(new { data = orders });
        }
        #endregion
    }
}
