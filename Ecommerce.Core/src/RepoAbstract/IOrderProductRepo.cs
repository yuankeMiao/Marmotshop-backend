

using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IOrderProductRepo
    {
        Task<List<OrderProduct>> GetAllOrderProductsByOrderIdAsync(Guid orderId);
        Task<OrderProduct> CreateOrderProductAsync (OrderProduct product);
    }
}