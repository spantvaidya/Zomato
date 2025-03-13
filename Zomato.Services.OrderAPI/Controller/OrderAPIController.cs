using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Zomato.Services.OrderAPI.Data;
using Zomato.Services.OrderAPI.Models;
using Zomato.Services.OrderAPI.Models.Dto;
using Zomato.Services.OrderAPI.Service.Interface;
using Zomato.Services.OrderAPI.Utility;
using System.Collections;

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

        public OrderAPIController(IMapper mapper, AppDbContext dbContext, IProductService productService)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _productService = productService;
            _responseDto = new ResponseDto();
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
    }
}
