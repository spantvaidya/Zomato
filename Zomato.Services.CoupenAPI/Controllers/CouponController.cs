﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zomato.Services.CoupenAPI.Data;
using Zomato.Services.CoupenAPI.Models;
using Zomato.Services.CoupenAPI.Models.Dto;
using Zomato.Services.CoupenAPI.Utility;

namespace Zomato.Services.CoupenAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;
        public CouponController(AppDbContext dbContext, IMapper mapper)
        {
            _dbcontext = dbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public object GetCoupons()
        {
            var objCoupens = _dbcontext.Coupons.ToList();
            _responseDto.Result = _mapper.Map<List<CouponDto>>(objCoupens);
            return _responseDto;
        }

        [HttpGet]
        [Route("GetCouponById/{id}")]
        public object GetCouponById(int id)
        {
            try
            {
                var coupen = _dbcontext.Coupons.FirstOrDefault(x => x.CouponId == id);
                _responseDto.Result = _mapper.Map<CouponDto>(coupen);
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
        [Route("GetCouponByCode/{code}")]
        public object GetCouponByCode(string code)
        {
            try
            {
                var coupen = _dbcontext.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
                _responseDto.Result = _mapper.Map<CouponDto>(coupen);
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
        [Route("AddCoupon")]
        [Authorize(Roles = SD.RoleAdmin)]
        public object AddCoupon([FromBody] CouponDto coupenDto)
        {
            try
            {
                var coupen = _mapper.Map<Coupon>(coupenDto);
                _dbcontext.Coupons.Add(coupen);
                _dbcontext.SaveChanges();

                //Cretae Coupon on Stripe
                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(coupen.DiscountAmount * 100),
                    Duration = "forever",
                    Id = coupen.CouponCode,
                    Currency = "usd"
                };

                var service = new Stripe.CouponService();
                var response = service.Create(options);

                _responseDto.Result = _mapper.Map<CouponDto>(coupen);
                _responseDto.StatusCode = Ok().StatusCode;
                return _responseDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }

        [HttpPut]
        [Route("UpdateCoupon")]
        [Authorize(Roles = SD.RoleAdmin)]
        public object UpdateCoupon([FromBody] CouponDto coupenDto)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(coupenDto);
                if (coupon != null)
                {
                    _dbcontext.Coupons.Update(coupon);
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
        [Route("DeleteCoupon/{Id}")]
        [Authorize(Roles = SD.RoleAdmin)]
        public object DeleteCoupon(int Id)
        {
            try
            {
                var coupon = _dbcontext.Coupons.FirstOrDefault(x => x.CouponId == Id);
                if (coupon != null)
                {
                    _dbcontext.Coupons.Remove(coupon);
                    _dbcontext.SaveChanges();

                    var service = new Stripe.CouponService();
                    var response = service.Delete(coupon.CouponCode);

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
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccess = false;
            }

            return _responseDto;
        }
    }
}
