using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Entity
{
    public class Order : BaseEntity
    {
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public required User User { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required ICollection<OrderProduct> Products { get; set; }
    }
}