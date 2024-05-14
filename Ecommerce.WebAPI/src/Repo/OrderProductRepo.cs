
using System.Collections.Immutable;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class OrderProductRepo : IOrderProductRepo
    {
        // this repo desen't have delete method, becasue by design, when the order is deleted, 
        // all related orderProducts will be deleted cascade
        // otherwise users and admins cannot changed order info - to simplify the logic
        private readonly AppDbContext _context;
        private readonly DbSet<OrderProduct> _orderProducts;
        public OrderProductRepo(AppDbContext context)
        {
            _context = context;
            _orderProducts = _context.OrderProducts;
        }
        public async Task<List<OrderProduct>> GetAllOrderProductsByOrderIdAsync(Guid orderId)
        {
            var orderProducts = await _orderProducts.Where(op => op.OrderId == orderId).ToListAsync();
            return orderProducts;
        }

        public async Task<OrderProduct> CreateOrderProductAsync(OrderProduct newOrderProduct)
        {
            var duplicatedOrderProduct = await _orderProducts.FirstOrDefaultAsync(op => op.OrderId == newOrderProduct.OrderId && op.ProductId == newOrderProduct.ProductId);
            if (duplicatedOrderProduct is not null) throw AppException.DuplicateException("Product in order");

            await _orderProducts.AddAsync(newOrderProduct);
            await _context.SaveChangesAsync();
            return newOrderProduct;
        }
    }
}