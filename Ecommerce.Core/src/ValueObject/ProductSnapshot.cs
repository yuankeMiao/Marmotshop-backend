
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.src.ValueObject
{
    // this snapshot is used to store immutable product data for orderProduct table
    // so when admin changed product, it will not influence the order info
    public class ProductSnapshot
    {
        [Key]
        public Guid Id { get; set; } // it will be the same with real product id
        public required string Title { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercentage { get; set; }
        public required string Thumbnail { get; set; }
    }
}