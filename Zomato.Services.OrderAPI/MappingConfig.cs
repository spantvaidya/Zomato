﻿using AutoMapper;
using Zomato.Services.OrderAPI.Models;
using Zomato.Services.OrderAPI.Models.Dto;

namespace Zomato.Services.OrderAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ProductDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, opt => opt.MapFrom(src => src.OrderTotal)).ReverseMap();

            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductDto.Name))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductDto.Price));

            CreateMap<OrderDetailsDto, CartDetailsDto>().ReverseMap();

            CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
        }
    }
}
