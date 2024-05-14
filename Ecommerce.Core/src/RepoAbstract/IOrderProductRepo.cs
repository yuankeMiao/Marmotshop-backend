

using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IOrderProductRepo
    {
        Task<OrderProduct> CreateOrderProduct (OrderProduct product);
        Task<bool> DeleteOrderProduct (Guid OrderId, Guid ProductId);
    }
}