using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IOrderService
    {
        Task<QueryResult<OrderReadDto>> GetAllOrdersAsync(OrderQueryOptions? options);
        Task<QueryResult<OrderReadDto>> GetAllOrdersByUserIdAsync(Guid userId, OrderQueryOptions? options); // Admin auth
        Task<OrderReadDto> GetOrderByIdAsync(Guid orderId);
        Task<OrderReadDto> CreateOrderWithTransactionAsync(Guid userId, OrderCreateDto orderCreateDto);
        Task<OrderReadDto> UpdateOrderByIdAsync(Guid orderId, OrderUpdateDto orderUpdateDto);
        Task<bool> DeleteOrderByIdAsync(Guid orderId);
    }
}