using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync(OrderQueryOptions? options);
        Task<IEnumerable<OrderReadDto>> GetAllOrdersByUserIdAsync(Guid userId, OrderQueryOptions? options); // Admin auth
        Task<OrderReadDto> GetOrderByIdAsync(Guid orderId);
        Task<OrderReadDto> CreateOrderWithtransactionAsync(Guid userId, OrderCreateDto orderCreateDto);
        Task<OrderReadDto> UpdateOrderByIdAsync(Guid orderId, OrderUpdateDto orderUpdateDto);
        Task<bool> DeleteOrderByIdAsync(Guid orderId);
    }
}