using AutoMapper;
using Zomato.Services.CartAPI.Models;
using Zomato.Services.CartAPI.Models.Dto;

namespace Zomato.Services.CartAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        }
    }
}
