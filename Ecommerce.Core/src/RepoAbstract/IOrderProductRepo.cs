

using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IOrderProductRepo
    {
        Task<HashSet<OrderProduct>> GetAllOrderProductsByOrderIdAsync(Guid orderId);
        Task<OrderProduct> CreateOrderProductAsync (OrderProduct product);
        Task<bool> DeleteOrderProductAsync (Guid OrderId, Guid ProductId);
    }
}