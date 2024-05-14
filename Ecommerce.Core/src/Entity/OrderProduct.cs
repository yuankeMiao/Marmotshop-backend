using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Entity
{
    public class OrderProduct
    {
        public Guid Id { get; set; }
        [ForeignKey("OrderId")]
        public Guid OrderId { get; set; } // Foreign key navigate to Order
        public Order Order { get; set; } = null!; // reference
        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; } // Foreign key navigate to product
        public ProductSnapshot ProductSnapshot { get; set; } = null!; //reference
        [Required]
        public int Quantity { get; set; }
    }
}