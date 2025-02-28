using AutoMapper;
using Zomato.Services.ProductAPI.Models;
using Zomato.Services.ProductAPI.Models.Dto;

namespace Zomato.Services.ProductAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
