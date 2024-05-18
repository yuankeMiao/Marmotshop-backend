using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ProductReadDto : BaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal? Rating { get; set; }
        public int Stock { get; set; }
        public string? Brand { get; set; }
        public required Guid CategoryId { get; set; }
        public required string Thumbnail { get; set; }
        public IEnumerable<ImageReadDto>? Images { get; set; }
    }

    public class ProductCreateDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercentage { get; set; }
        public int Stock { get; set; }
        public string? Brand { get; set; }
        public Guid CategoryId { get; set; }
        public required string Thumbnail { get; set; }
        public IEnumerable<ImageCreateDto>? Images { get; set; }
    }


    public class ProductUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? DiscountPercentage { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Stock { get; set; }
        public string? Brand { get; set; }
        public string? Thumbnail { get; set; }
        public IEnumerable<ImageUpdateDto>? Images { get; set; }
    }
}