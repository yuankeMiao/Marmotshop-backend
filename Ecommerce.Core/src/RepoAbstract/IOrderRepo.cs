using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IOrderRepo
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(OrderQueryOptions? options); // Admin auth
        Task<IEnumerable<Order>> GetAllOrdersByUserIdAsync(Guid userId, OrderQueryOptions? options); // Admin auth
        Task<Order> GetOrderByIdAsync(Guid orderId); // Admin auth
        Task<Order> CreateOrderWithTransactionAsync(Order createdOrder); // Customer auth
        Task<Order> UpdateOrderByIdAsync(Order updatedOrder); // Admin auth
        Task<bool> DeleteOrderByIdAsync(Guid orderId); // Admin auth
    }
}