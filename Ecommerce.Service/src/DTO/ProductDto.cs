using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    // I removed image dto, since it is a simple logic and I only need url
    // so for updating, if ImageUrls is not null, i will remove all existing images and re-create all iamges
    // then I don't need to pass image Id to client
    public class ProductReadDto : BaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int Price { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal? Rating { get; set; }
        public int Stock { get; set; }
        public string? Brand { get; set; }
        public required Guid CategoryId { get; set; }
        public required CategoryReadDto Category { get; set; }
        public required string Thumbnail { get; set; }
        public required ICollection<string> ImageUrls { get; set; }
    }

    public class ProductCreateDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int Price { get; set; }
        public int DiscountPercentage { get; set; }
        public int Stock { get; set; }
        public string? Brand { get; set; }
        public Guid CategoryId { get; set; }
        public required string Thumbnail { get; set; }
        public required ICollection<string> ImageUrls { get; set; }
    }


    public class ProductUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public int? DiscountPercentage { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Stock { get; set; }
        public string? Brand { get; set; }
        public string? Thumbnail { get; set; }
        public ICollection<string>? ImageUrls { get; set; }
    }
}