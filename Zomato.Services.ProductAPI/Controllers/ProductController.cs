using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zomato.Services.ProductAPI.Data;
using Zomato.Services.ProductAPI.Models;
using Zomato.Services.ProductAPI.Models.Dto;
using Zomato.Services.ProductAPI.Utility;

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
        //[Authorize]
        public object GetProducts()
        {
            var objProducts = _dbcontext.Products.ToList();
            _responseDto.Result = _mapper.Map<List<ProductDto>>(objProducts);
            return _responseDto;
        }

        [HttpGet]
        [Route("GetProductById/{id}")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize(Roles = SD.RoleAdmin)]
        public object AddProduct(ProductDto ProductDto)
        {
            try
            {
                var Product = _mapper.Map<Product>(ProductDto);
                _dbcontext.Products.Add(Product);
                _dbcontext.SaveChanges();

                if (ProductDto != null)
                {
                    string filename = Product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    string filepath = @"wwwroot/ProductImages/" + filename;
                    using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                    Product.ImageLocalPath = filepath;
                    Product.ImageUrl = baseUrl + @"/ProductImages/" + filename;
                }
                else
                {
                    Product.ImageUrl = "https://placehold.co/600x400";
                }

                _dbcontext.Products.Update(Product);
                _dbcontext.SaveChanges();

                _responseDto.Result = _mapper.Map<ProductDto>(Product);
                _responseDto.StatusCode = Ok().StatusCode;
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.InnerException?.Message ?? ex.Message;
                _responseDto.IsSuccess = false;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
                return StatusCode(_responseDto.StatusCode, _responseDto);
            }

            return _responseDto;
        }

        [HttpPut]
        [Route("UpdateProduct")]
        [Authorize(Roles = SD.RoleAdmin)]
        public object UpdateProduct(ProductDto ProductDto)
        {
            try
            {
                var Product = _mapper.Map<Product>(ProductDto);
                if (Product != null)
                {
                    if (ProductDto.Image != null)
                    {
                        //delete old image file
                        if (!string.IsNullOrEmpty(Product.ImageLocalPath))
                        {
                            var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(),Product.ImageLocalPath);
                            FileInfo oldFile = new FileInfo(oldFilePathDirectory);
                            if (oldFile.Exists)
                            {
                                oldFile.Delete();
                            }
                        }

                        //update new image
                        string filename = Product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                        string filepath = @"wwwroot/ProductImages/" + filename;
                        using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            ProductDto.Image.CopyTo(fileStream);
                        }

                        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                        Product.ImageLocalPath = filepath;
                        Product.ImageUrl = baseUrl + @"/ProductImages/" + filename;
                    }

                    _dbcontext.Products.Update(Product);
                    _dbcontext.SaveChanges();
                    _responseDto.Result = Product;
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
        [Authorize(Roles = SD.RoleAdmin)]
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
