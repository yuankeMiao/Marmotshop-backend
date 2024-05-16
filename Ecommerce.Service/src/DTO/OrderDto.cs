using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{
    public class OrderReadDto : BaseEntity
    {
        public required UserReadDto User { get; set; } // User information
        public required HashSet<OrderProductReadDto> Products { get; set; }
        public OrderStatus Status { get; set; }
        public required string ShippingAddress { get; set; }
    }

    public class OrderCreateDto
    {
        public required HashSet<OrderProductCreateDto> Products { get; set; }
        public required string ShippingAddress { get; set; }

    }

    public class OrderUpdateDto // to simplify the logic, only update status
    {
        public OrderStatus Status { get; set; }
    }
}