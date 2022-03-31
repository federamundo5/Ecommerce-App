using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.OrderAggregate;

namespace API.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
            .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
             .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>())
             .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name));

             CreateMap<Address,AddressDto>().ReverseMap();

             CreateMap<CustomerBasketDto,CustomerBasket>().ReverseMap();
             CreateMap<BasketItemDto,BasketItem>().ReverseMap();
             CreateMap<OrderAddress,AddressDto>().ReverseMap();
             CreateMap<Order, OrderToReturnDto>()
             .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
             .ForMember(d => d.shippingPrice, o => o.MapFrom(s => s.DeliveryMethod.Price));


              CreateMap<OrderItem, OrderItemDto>()
               .ForMember(d => d.ProductId, o => o.MapFrom(s => s.itemOrdered.ProductItemId))
              .ForMember(d => d.ProductName, o => o.MapFrom(s => s.itemOrdered.ProductName))
             .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.itemOrdered.PictureUrl))
             .ForMember(d=> d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());






        }
    }
}