using AutoMapper;
using Ecommerce.Core.src.Entity;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.Shared
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region Category Mapper:
            CreateMap<Category, CategoryReadDto>();

            CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<CategoryUpdateDto, Category>();
            #endregion

            #region User Mapper:
            CreateMap<UserCreateDto, User>();

            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UserUpdateDto, User>();
            #endregion


            #region Product Mapper:
            CreateMap<Product, ProductReadDto>();

            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<ProductUpdateDto, Product>();
            #endregion


            #region Order Mapper:
            CreateMap<Order, OrderReadDto>();

            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<OrderUpdateDto, Order>();

            #endregion

            #region Order Product Mapper:
            CreateMap<OrderProduct, OrderProductReadDto>();
            CreateMap<Product, OrderProduct>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            #endregion

            #region Review mapper
            CreateMap<Review, ReviewReadDto>();

            CreateMap<ReviewCreateDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore());

            CreateMap<ReviewUpdateDto, Review>();
            #endregion
        }
    }
}
