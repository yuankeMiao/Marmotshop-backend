using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class OrderProduct
    {
        [ForeignKey("OrderId")]
        public Guid OrderId { get; set; } // Foreign key navigate to Order

        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; } // Foreign key navigate to product
        public required Product Product { get; set; }
        public int Quantity { get; set; }
    }
}