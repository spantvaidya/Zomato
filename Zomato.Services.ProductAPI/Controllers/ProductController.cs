using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Zomato.Services.ProductAPI.Data;
using Zomato.Services.ProductAPI.Models;
using Zomato.Services.ProductAPI.Models.Dto;

namespace Zomato.Services.ProductAPI.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;
        public ProductController(AppDbContext dbContext, IMapper mapper)
        {
            _dbcontext = dbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        public object GetProducts()
        {
            var objProducts = _dbcontext.Products.ToList();
            _responseDto.Result = _mapper.Map<List<ProductDto>>(objProducts);
            return _responseDto;
        }

        [HttpGet]
        [Route("GetProductById/{id}")]
        public object GetProductById(int id)
        {
            try
            {
                var Product = _dbcontext.Products.FirstOrDefault(x => x.ProductId == id);
                _responseDto.Result = _mapper.Map<ProductDto>(Product);
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Message = "failure";
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }

        [HttpGet]
        [Route("GetProductByName/{name}")]
        public object GetProductByName(string name)
        {
            try
            {
                var Product = _dbcontext.Products.Where(x => x.Name.ToLower().Contains(name)).ToList();
                _responseDto.Result = _mapper.Map<List<ProductDto>>(Product);
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Message = "failure";
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }

        [HttpGet]
        [Route("GetProductByCategory/{Category}")]
        public object GetProductByCategory(string Category)
        {
            try
            {
                var Product = _dbcontext.Products.Where(x => x.Category.ToLower().Contains(Category)).ToList();
                _responseDto.Result = _mapper.Map<List<ProductDto>>(Product);
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Message = "failure";
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }

        [HttpPost]
        [Route("AddProduct")]
        public object AddProduct([FromBody] ProductDto ProductDto)
        {
            try
            {
                var Product = _mapper.Map<Product>(ProductDto);
                _dbcontext.Products.Add(Product);
                _dbcontext.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDto>(Product);
                _responseDto.StatusCode = Ok().StatusCode;
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.InnerException.Message;
                _responseDto.Message = "failure";
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }

        [HttpPut]
        [Route("UpdateProduct")]
        public object UpdateProduct([FromBody] ProductDto ProductDto)
        {
            try
            {
                var Product = _mapper.Map<Product>(ProductDto);
                if (Product != null)
                {
                    _dbcontext.Products.Update(Product);
                    _dbcontext.SaveChanges();
                    _responseDto.Result = Ok();
                    _responseDto.StatusCode = Ok().StatusCode;
                }

                else
                {
                    _responseDto.StatusCode = NotFound().StatusCode;
                    _responseDto.Message = "Record not found";
                    _responseDto.IsSuccess = false;
                }

                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.InnerException.Message;
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }

        [HttpPost]
        [Route("DeleteProduct/{Id}")]
        public object DeleteProduct(int Id)
        {
            try
            {
                var Product = _dbcontext.Products.FirstOrDefault(x => x.ProductId == Id);
                if (Product != null)
                {
                    _dbcontext.Products.Remove(Product);
                    _dbcontext.SaveChanges();
                    _responseDto.Result = Ok();
                    _responseDto.StatusCode = Ok().StatusCode;
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Record not found";
                    _responseDto.StatusCode = NotFound().StatusCode;
                }
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = "failure";
            }

            return _responseDto;
        }
    }
}
