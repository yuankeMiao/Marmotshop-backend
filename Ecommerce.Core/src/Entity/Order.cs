using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Entity
{
    public class Order : BaseEntity
    {
        [ForeignKey("UserId")]
        public Guid UserId { get; set; } // foreign key
        public User User { get; set; } = null!; //reference only

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public HashSet<OrderProduct> Products { get; set; } = null!;

        public required string ShippingAddress { get; set; }
    }
}