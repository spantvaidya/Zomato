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

        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderDetails = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                return View(orderDetails);
            }
            return NotFound();
        }

        [HttpPost("ReadyForPickup")]
        public async Task<IActionResult> ReadyForPickup(int orderId)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Status Updated Successfully";
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Status Updated Successfully";
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }
            else
            {
                TempData["error"] = response.Message;
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }           
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Status Updated Successfully";
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }
            return View();
        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeaderDto> orders;
            string userId = string.Empty;
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            //if (!User.IsInRole(SD.RoleAdmin))
            userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto response = _orderService.GetAllOrders(userId).GetAwaiter().GetResult();
            if (response != null && response.IsSuccess)
            {
                orders = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDto>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "Pending":
                        orders = orders.Where(x => x.OrderStatus == SD.Status_Pending);
                        break;
                    case "Approved":
                        orders = orders.Where(x => x.OrderStatus == SD.Status_Approved);
                        break;
                    case "ReadyForPickup":
                        orders = orders.Where(x => x.OrderStatus == SD.Status_ReadyForPickup);
                        break;
                    case "Completed":
                        orders = orders.Where(x => x.OrderStatus == SD.Status_Completed);
                        break;
                    case "Cancelled":
                        orders = orders.Where(x => x.OrderStatus == SD.Status_Cancelled || x.OrderStatus == SD.Status_Refunded);
                        break;
                    default:
                        break;
                }
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
