
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Product : BaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercentage { get; set; } = 0;
        public decimal? Rating { get; set; } // everytime a new review is added, it will trigger a function to calculate and change this number
        public int Stock { get; set; } = 0;
        public string? Brand { get; set; }
        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; } //  foreign key navigate to category
        public required Category Category{ get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = [];
    }
}