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

        public async Task<QueryResult<Order>> GetAllOrdersAsync(OrderQueryOptions? options)
        {
            var query = _orders.AsQueryable();
            query = query.Include(o => o.Products);

            if (options is not null)
            {
                ApplyQueryOptions(query, options);

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                // Pagination
                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }

                var orders = await query.ToListAsync();
                return new QueryResult<Order> { Data = orders, TotalCount = totalCount };
            }
            else
            {
                var orders = await query.ToListAsync();
                return new QueryResult<Order> { Data = orders, TotalCount = orders.Count };
            }
        }

        public async Task<QueryResult<Order>> GetAllOrdersByUserIdAsync(Guid userId, OrderQueryOptions? options)
        {
            var query = _orders.AsQueryable();
            query = query.Include(o => o.Products);
            query = query.Where(o => o.UserId == userId);

            if (options is not null)
            {
                ApplyQueryOptions(query, options);

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                // Pagination
                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }

                var orders = await query.ToListAsync();
                return new QueryResult<Order> { Data = orders, TotalCount = totalCount };
            }
            else
            {
                var orders = await query.ToListAsync();
                return new QueryResult<Order> { Data = orders, TotalCount = orders.Count };
            }

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
                    if (duplicatedOrderProduct is not null) throw AppException.Duplicate("Product in order");

                    await _orderProducts.AddAsync(newOrderProduct);

                    // change product.Stock in product table
                    var foundProduct = await _products.FindAsync(newOrderProduct.ProductId) ?? throw AppException.NotFound("Product Not Found");
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

        private static IQueryable<Order> ApplyQueryOptions(IQueryable<Order> query, OrderQueryOptions options)
        {
            if (options.Status is not null)
            {
                query = query.Where(o => o.Status == options.Status);
            }

            // Sorting
            if (!string.IsNullOrEmpty(options.SortBy))
            {
                query = options.SortBy.ToLower() switch
                {
                    "created_date" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
                    "updated_date" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.UpdatedDate) : query.OrderBy(p => p.UpdatedDate),
                    _ => query.OrderBy(p => p.CreatedDate),
                };
            }

            return query;
        }

    }
}