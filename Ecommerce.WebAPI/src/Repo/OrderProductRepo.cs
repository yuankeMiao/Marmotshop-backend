
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;

namespace Ecommerce.WebAPI.src.Repo
{
    public class OrderProductRepo : IOrderProductRepo
    {
        public Task<OrderProduct> CreateOrderProduct(OrderProduct product)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteOrderProduct(Guid OrderId, Guid ProductId)
        {
            throw new NotImplementedException();
        }

    }
}