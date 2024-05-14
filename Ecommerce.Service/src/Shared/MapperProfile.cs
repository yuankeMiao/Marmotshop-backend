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
            CreateMap<Category, CategoryReadDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryImage, opt => opt.MapFrom(src => src.Image));

            CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CategoryImage));

            CreateMap<CategoryUpdateDto, Category>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CategoryImage));
            #endregion

            #region User Mapper:
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.UserFirstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.UserLastname))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.UserPassword))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.UserAvatar))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole))
                 .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.UserFirstname, opt => opt.MapFrom(src => src.Lastname))
                .ForMember(dest => dest.UserLastname, opt => opt.MapFrom(src => src.Firstname))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole));

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.UserFirstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.UserLastname))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.UserPassword))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.UserAvatar))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole));
            #endregion


            #region Product Mapper:
            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.ProductDiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.ProductRating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.ProductStock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.ProductThumbnail, opt => opt.MapFrom(src => src.Thumbnail))
                .ForMember(dest => dest.ProductImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate));

            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ProductTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDescription))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.ProductDiscountPercentage))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.ProductStock))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.ProductBrand))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.ProductThumbnail))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ProductImageUrls));

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ProductTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDescription))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.ProductDiscountPercentage))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.ProductStock))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.ProductBrand))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.ProductThumbnail))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ProductImageUrls));

            // modify later
            // CreateMap<Product, ProductUpdateDto>()
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //     .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Title))
            //     .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
            //     .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
            //     .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            //     .ForMember(dest => dest.ProductInventory, opt => opt.MapFrom(src => src.Inventory));

            #endregion


            #region Order Mapper:
            CreateMap<Order, OrderReadDto>()
                .ForMember(dest => dest.OrderUser, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OrderAddressSnapshot, opt => opt.MapFrom(src => src.AddressSnapshot));

            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.OrderUserId))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.AddressSnapshot, opt => opt.MapFrom(src => src.OrderAddressSnapshot));

            CreateMap<OrderUpdateDto, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus));
            
            #endregion

            #region Order Product Mapper:
            CreateMap<OrderProduct, OrderProductReadDto>()
                .ForMember(dest => dest.ProductQuantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.ProductSnapshot.Title))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.ProductSnapshot.Price))
                .ForMember(dest => dest.ProductDiscountPercentage, opt => opt.MapFrom(src => src.ProductSnapshot.DiscountPercentage));

            CreateMap<OrderProductCreateDto, OrderProduct>();

            #endregion

            #region Review mapper
            CreateMap<Review, ReviewReadDto>()
                .ForMember(dest => dest.ReviewRating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.ReviewContent, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.ReviewUser, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.ReviewProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ReviewProductTitle, opt => opt.MapFrom(src => src.Product.Title));

            CreateMap<ReviewCreateDto, Review>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ReviewRating))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.ReviewContent))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore());

            CreateMap<ReviewUpdateDto, Review>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ReviewRating))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.ReviewContent));
            #endregion
        }
    }
}

// Will be modified during the time we use them.