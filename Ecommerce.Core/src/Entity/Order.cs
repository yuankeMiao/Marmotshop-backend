using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Entity
{
    public class Order : BaseEntity
    {
        [ForeignKey("UserId")]
        public Guid UserId { get; set; } // foreign key
        public User User { get; set; } = null!; //reference
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required ICollection<OrderProduct> Products { get; set; }
        // I removed foreign key reference for address, becaue if user deleted an address, 
        //it should not influence the address in orders
        // now when user place an order, it will pass the value of address to order
        public required Address Address{ get; set; } 
    }
}