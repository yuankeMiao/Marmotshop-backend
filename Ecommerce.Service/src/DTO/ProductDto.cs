using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    // I removed image dto, since it is a simple logic and I only need url
    // so for updating, if ImageUrls is not null, i will remove all existing images and re-create all iamges
    // then I don't need to pass image Id to client
    public class ProductReadDto : BaseEntity
    {
        public required string ProductTitle { get; set; }
        public required string ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public int ProductDiscountPercentage { get; set; }
        public decimal? ProductRating { get; set; }
        public int ProductStock { get; set; }
        public string? ProductBrand { get; set; }
        public required CategoryReadDto ProductCategory { get; set; }
        public required string ProductThumbnail { get; set; }
        public required ICollection<string> ProductImageUrls { get; set; }
    }

    public class ProductCreateDto
    {
        public required string ProductTitle { get; set; }
        public required string ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public int ProductDiscountPercentage { get; set; }
        public int ProductStock { get; set; }
        public string? ProductBrand { get; set; }
        public Guid CategoryId { get; set; }
        public required string ProductThumbnail { get; set; }
        public required ICollection<string> ProductImageUrls { get; set; }
    }


    public class ProductUpdateDto
    {
        public string? ProductTitle { get; set; }
        public string? ProductDescription { get; set; }
        public int? ProductPrice { get; set; }
        public int? ProductDiscountPercentage { get; set; }
        public Guid? CategoryId { get; set; }
        public int? ProductStock { get; set; }
        public string? ProductBrand { get; set; }
        public string? ProductThumbnail { get; set; }
        public ICollection<string>? ProductImageUrls { get; set; }
    }
}