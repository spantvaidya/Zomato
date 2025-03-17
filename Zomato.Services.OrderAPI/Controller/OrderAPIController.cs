using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Zomato.MessageBus;
using Zomato.Services.OrderAPI.Data;
using Zomato.Services.OrderAPI.Models;
using Zomato.Services.OrderAPI.Models.Dto;
using Zomato.Services.OrderAPI.Service.Interface;
using Zomato.Services.OrderAPI.Utility;

namespace Zomato.Services.OrderAPI.Controller
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private IProductService _productService;
        private IConfiguration _configuration;
        private IMessageBus _messageBus;

        public OrderAPIController(IMapper mapper, AppDbContext dbContext, IProductService productService,
            IConfiguration configuration, IMessageBus messageBus)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _productService = productService;
            _configuration = configuration;
            _messageBus = messageBus;
            _responseDto = new ResponseDto();
        }

        [HttpGet("GetOrders")]
        [Authorize]
        public ResponseDto? GetOrders(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    orderHeaders = _dbContext.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(x => x.OrderHeaderId).ToList();
                }
                else
                    orderHeaders = _dbContext.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).ToList();

                IEnumerable<OrderHeaderDto> orderHeaderDtos = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);

                foreach (var item in orderHeaderDtos)
                {
                    item.OrderDetailsDto = _mapper.Map<IEnumerable<OrderDetailsDto>>(_dbContext.OrderDetails.Where(u => u.OrderHeaderId == item.OrderHeaderId));
                }
                _responseDto.Result = orderHeaderDtos;
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }

        [HttpGet("GetOrder/{id:int}")]
        [Authorize]
        public ResponseDto? GetOrder(int id)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeaders.Include(u => u.OrderDetails).First(x => x.OrderHeaderId == id);
                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }

        [HttpPost("CreateOrder")]
        [Authorize]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderCreated = DateTime.Now;
                orderHeaderDto.OrderStatus = SD.Status_Pending;

                orderHeaderDto.OrderDetailsDto = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.Cartdetails);

                OrderHeader orderHeaderCreated = _dbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _dbContext.SaveChangesAsync();

                //Create OrderDetails
                foreach (var item in orderHeaderDto.OrderDetailsDto)
                {
                    item.OrderHeaderId = orderHeaderCreated.OrderHeaderId;
                    item.Price = item.ProductDto.Price;
                    item.ProductName = item.ProductDto.Name;
                    _dbContext.OrderDetails.Add(_mapper.Map<OrderDetails>(item));
                }
                await _dbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderHeaderCreated.OrderHeaderId;
                _responseDto.Result = orderHeaderDto;
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }

        [HttpPost("CreateStripeSession")]
        [Authorize]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                // This example sets up an endpoint using the ASP.NET MVC framework.
                // Watch this video to get started: https://youtu.be/2-mMOB8MhmE.  
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl
                };

                var discountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetailsDto)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.Price * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                //Apply Coupon only if Discount is greater than 0
                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountsObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);

                stripeRequestDto.StripeSessionUrl = session.Url;

                //Save StripeSessionId
                OrderHeader orderHeader = _dbContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _dbContext.SaveChanges();


                _responseDto.Result = stripeRequestDto;
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }

        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                //Save PaymentIntentId
                OrderHeader orderHeader = _dbContext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.OrderStatus = SD.Status_Approved;
                    _dbContext.SaveChanges();

                    //Publish Rewards points message to serviceBus
                    RewardsDto rewardsDto = new RewardsDto
                    {
                        UserId = orderHeader.UserId,
                        Points = Convert.ToInt32(orderHeader.OrderTotal / 2),
                        OrderId = orderHeader.OrderHeaderId,
                        Email = orderHeader.Email
                    };
                    string rewardsTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDto, rewardsTopic);
                }

                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeaders.First(u => u.OrderHeaderId == orderId);

                if (newStatus == SD.Status_Cancelled)
                {
                    //Refund the amount
                    var service = new RefundService();
                    var refundOptions = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId,
                    };
                    Refund refund = service.Create(refundOptions);
                }

                orderHeader.OrderStatus = newStatus;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }
    }
}
