using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Product Product { get; set; } = null!; //reference
        [Required]

        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public required decimal ActualPrice { get; set; }
        public int Quantity { get; set; }
        public required decimal TotalPrice { get; set; }



        // to make sure in a same order, no duplicated OrderPorduct items
        public override bool Equals(object? obj)
        {
            if (obj is OrderProduct orderProduct)
            {
                if (OrderId == orderProduct.OrderId && ProductId == orderProduct.ProductId)
                {
                    return true;
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, ProductId);
        }
    }
}