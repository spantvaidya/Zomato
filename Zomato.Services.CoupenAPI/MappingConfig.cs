using AutoMapper;
using Zomato.Services.CoupenAPI.Models;

namespace Zomato.Services.CoupenAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
