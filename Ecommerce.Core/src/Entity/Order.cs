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
        // I removed reference of address, so the order address will never change
        // otherwise when user changed address, it will influence the order history
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int AddressPostalCode { get; set; }
        public required string AddressCity { get; set; }

    }
}