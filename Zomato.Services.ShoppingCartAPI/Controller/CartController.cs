using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zomato.MessageBus;
using Zomato.Services.CartAPI.Data;
using Zomato.Services.CartAPI.Models;
using Zomato.Services.CartAPI.Models.Dto;
using Zomato.Services.CartAPI.Service.Interface;

namespace Zomato.Services.CartAPI.Controller
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly ResponseDto _responseDto;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public CartController(IMapper mapper, AppDbContext dbContext, IProductService productService,
            ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _responseDto = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var cartHeader = _mapper.Map<CartHeaderDto>
                    (
                        await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId)
                    );
                var cartDetails = _mapper.Map<List<CartDetailsDto>>
                    (
                        await _dbContext.CartDetails.Where(x => x.CartHeaderId == cartHeader.CartHeaderId)
                        .ToListAsync()
                    );

                //Get Products associated with the Cart
                IEnumerable<ProductDto> products = await _productService.GetAllProducts();

                foreach (var item in cartDetails)
                {
                    item.ProductDto = products.FirstOrDefault(x => x.ProductId == item.ProductId);
                    var itemTotal = item.ProductDto.Price * item.Count;

                    cartHeader.CartTotal += itemTotal;
                }

                //Get Coupons associated with the Cart
                if (cartHeader.CouponCode != null)
                {
                    CouponDto coupon = await _couponService.GetCouponByCode(cartHeader.CouponCode);
                    if (coupon != null && !String.IsNullOrEmpty(coupon.CouponCode) && cartHeader.CartTotal > (double)coupon.MinAmount)
                    {
                        cartHeader.CouponCode = coupon.CouponCode;
                        cartHeader.Discount = (double)coupon.DiscountAmount;
                        cartHeader.CartTotal = cartHeader.CartTotal - (double)coupon.DiscountAmount;
                    }
                }

                CartDto cartDto = new CartDto
                {
                    CartHeader = cartHeader,
                    Cartdetails = cartDetails
                };
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Operation Successful";
                _responseDto.Result = cartDto;
                _responseDto.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert([FromBody] CartDto cartDto)
        {
            try
            {
                //check if cart exists
                var cartHeaderFromDb = await _dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //create cart
                    var cartHeaderCreate = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    await _dbContext.CartHeaders.AddAsync(cartHeaderCreate);
                    await _dbContext.SaveChangesAsync();
                    //create cart details
                    if (cartDto.Cartdetails != null && cartDto.Cartdetails.Any())
                    {
                        var cartDetail = cartDto.Cartdetails.First();
                        cartDetail.CartHeaderId = cartHeaderCreate.CartHeaderId;
                        await _dbContext.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDetail));
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    //check if details has same product
                    if (cartDto.Cartdetails != null && cartDto.Cartdetails.Any())
                    {
                        var cartDetail = cartDto.Cartdetails.First();
                        var cartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                            x => x.ProductId == cartDetail.ProductId &&
                            x.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                        if (cartDetailsFromDb == null)
                        {
                            //create cart details
                            cartDetail.CartHeaderId = cartHeaderFromDb.CartHeaderId;
                            await _dbContext.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDetail));
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            //update cart details i.e., update count in cart details
                            cartDetail.Count += cartDetailsFromDb.Count;
                            cartDetail.CartHeaderId = cartDetailsFromDb.CartHeaderId ?? 0;
                            cartDetail.CartDetailsId = cartDetailsFromDb.CartDetailsId;
                            _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDetail));
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Operation Successful";
                _responseDto.Result = cartDto;
                _responseDto.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("ClearCart")]
        public async Task<ResponseDto> ClearCart([FromBody] int cartDetailsId)
        {
            try
            {
                //check if cart exists
                CartDetails cartDetails = await _dbContext.CartDetails.FirstOrDefaultAsync(x => x.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _dbContext.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _dbContext.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    //remove cartHeader as user is removing last item in the cart
                    CartHeader cartHeader = _dbContext.CartHeaders.FirstOrDefault(x => x.CartHeaderId == cartDetails.CartHeaderId);

                    if (cartHeader != null)
                        _dbContext.CartHeaders.Remove(cartHeader);
                }
                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Operation Successful";
                _responseDto.Result = true;
                _responseDto.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }
        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _dbContext.CartHeaders.FirstAsync(x => x.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _dbContext.Update(cartFromDb);
                await _dbContext.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }
        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                //Send Email
                await _messageBus.PublishMessage(cartDto, _configuration.GetSection("TopicAndQueueNames:EmailShoppingCartQueue").Value);
                _responseDto.Result = true;
                _responseDto.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }
    }
}
