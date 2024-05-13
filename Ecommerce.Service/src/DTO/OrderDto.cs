using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{
    public class OrderReadDto : BaseEntity
    {
        public required UserReadDto OrderUser { get; set; } // User information
        public required ICollection<OrderProductReadDto> OrderProducts { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class OrderCreateDto
    {
        public Guid OrderUserId { get; set; }
        public required ICollection<OrderProductCreateDto> OrderProducts { get; set; }
    }

    public class OrderUpdateDto // to simplify the logic, only update status
    {
        public OrderStatus OrderStatus { get; set; }
    }

    // I think this is not needed, so i tempararily comment it out, will check later
    // public class OrderReadUpdateDto : BaseEntity
    // {
    //     public required UserReadDto User { get; set; } // User information
    //     public OrderStatus OrderStatus { get; set; }
    //     public required IEnumerable<OrderProductReadDto> OrderProducts { get; set; } // Order products list
    // }
}