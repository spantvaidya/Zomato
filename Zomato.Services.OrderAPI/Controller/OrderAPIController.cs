using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
