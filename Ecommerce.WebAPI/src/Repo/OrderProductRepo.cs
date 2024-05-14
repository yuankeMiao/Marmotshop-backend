
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;

namespace Ecommerce.WebAPI.src.Repo
{
    public class OrderProductRepo : IOrderProductRepo
    {
        public Task<HashSet<OrderProduct>> GetAllOrderProductsByOrderIdAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }
        public Task<OrderProduct> CreateOrderProductAsync(OrderProduct product)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteOrderProductAsync(Guid OrderId, Guid ProductId)
        {
            throw new NotImplementedException();
        }

    }
}