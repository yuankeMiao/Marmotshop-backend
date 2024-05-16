using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class OrderRepo : IOrderRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Order> _orders;
        private readonly DbSet<OrderProduct> _orderProducts;
        private readonly DbSet<Product> _products;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
            _orders = _context.Orders;
            _orderProducts = _context.OrderProducts;
            _products = _context.Products;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(BaseQueryOptions? options)
        {
            var query = _orders.AsQueryable();
            query = query.Include(o => o.Products);

            // Pagination
            if (options is not null)
            {
                query = query.OrderBy(o => o.CreatedDate)
                             .Skip(options.Offset)
                             .Take(options.Limit);
            }

            var orders = await query.ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            var foundOrder = await _orders.FindAsync(orderId) ?? throw AppException.NotFound("Order not found");
            var foundOrderProducts = await _orderProducts.Where(op => op.OrderId == foundOrder.Id).ToListAsync();
            foundOrder.Products = [.. foundOrderProducts];

            return foundOrder;
        }

        public async Task<Order> CreateOrderWithTransactionAsync(Order newOrder)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // create order record in order table
                await _orders.AddAsync(newOrder);

                // create orderProduct record in orderProduct table
                foreach (var newOrderProduct in newOrder.Products)
                {
                    var duplicatedOrderProduct = await _orderProducts.FirstOrDefaultAsync(op => op.OrderId == newOrderProduct.OrderId && op.ProductId == newOrderProduct.ProductId);
                    if (duplicatedOrderProduct is not null) throw AppException.DuplicateException("Product in order");

                    await _orderProducts.AddAsync(newOrderProduct);

                    // change product.Stock in product table
                    var foundProduct = await _products.FindAsync(newOrderProduct.ProductId) ?? throw AppException.NotFound("Product");
                    foundProduct.Stock -= newOrderProduct.Quantity;

                    _products.Update(foundProduct);
                }

                await _context.SaveChangesAsync();
                transaction.Commit();
                
                return newOrder;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Order> UpdateOrderByIdAsync(Order updatedOrder)
        {
            _orders.Update(updatedOrder);
            await _context.SaveChangesAsync();
            return updatedOrder;
        }

        public async Task<bool> DeleteOrderByIdAsync(Guid orderId)
        {
            var foundOrder = await _orders.FindAsync(orderId) ?? throw AppException.NotFound("Order not found for ID: " + orderId);
            _orders.Remove(foundOrder);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}