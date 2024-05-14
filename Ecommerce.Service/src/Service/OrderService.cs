using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using AutoMapper;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.Service
{
    public class OrderService : IOrderService
    {
        private IMapper _mapper;
        private readonly IOrderRepo _orderRepo;
        private IProductRepo _productRepo;
        private IUserRepo _userRepo;
        private IOrderProductRepo _orderProductRepo;

        public OrderService(IOrderRepo orderRepo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo, IOrderProductRepo orderProductRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _orderProductRepo = orderProductRepo;
        }

        public async Task<OrderReadDto> CreateOrderAsync(Guid userId, OrderCreateDto orderCreateDto)
        {
            // check if user exists
            _ = await _userRepo.GetUserByIdAsync(userId) ?? throw AppException.NotFound("User not found");

            // if true, created order object, assign userId
            var order = _mapper.Map<OrderCreateDto, Order>(orderCreateDto);
            order.UserId = userId;

            // create orderProducts set
            var newOrderProducts = new HashSet<OrderProduct>();

            foreach (var orderProductDto in orderCreateDto.Products)
            {
                // check every product in orderCreateDto.OrderProducts
                var foundProduct = await _productRepo.GetProductByIdAsync(orderProductDto.ProductId) ?? throw AppException.NotFound("Product not found");
                // if found, create a new OrderProduct object
                var newOrderProduct = _mapper.Map<Product, OrderProduct>(foundProduct);
                newOrderProduct.Quantity = orderProductDto.Quantity;
                newOrderProduct.TotalPrice = newOrderProduct.Quantity * newOrderProduct.ActualPrice;
                newOrderProduct.OrderId = order.Id;

                var createdOrderProduct = await _orderProductRepo.CreateOrderProductAsync(newOrderProduct);
                newOrderProducts.Add(createdOrderProduct);
            }
            order.Products = newOrderProducts;
            order.Status = OrderStatus.Pending;

            var createdOrder = await _orderRepo.CreateOrderAsync(order);
            var orderReadDto = _mapper.Map<OrderReadDto>(createdOrder);
            return orderReadDto;
        }

        public async Task<bool> DeleteOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                AppException.BadRequest("OrderId is required");
            }
            try
            {
                var targetOrder = await _orderRepo.GetOrderByIdAsync(orderId);
                if (targetOrder is not null)
                {
                    await _orderRepo.DeleteOrderByIdAsync(orderId);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync(BaseQueryOptions? options)
        {
            var orders = await _orderRepo.GetAllOrdersAsync(options);
            var orderReadDtos = new HashSet<OrderReadDto>();

            foreach (var order in orders)
            {
                var orderReadDto = _mapper.Map<Order, OrderReadDto>(order);
                var orderProdutcs = await _orderProductRepo.GetAllOrderProductsByOrderIdAsync(order.Id);
                orderReadDto.Products = _mapper.Map<HashSet<OrderProduct>, HashSet<OrderProductReadDto>>(orderProdutcs);
                orderReadDtos.Add(orderReadDto);
            }

            return orderReadDtos;
        }

        public async Task<OrderReadDto> GetOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                AppException.BadRequest("OrderId is required");
            }
            try
            {
                var foundOrder = await _orderRepo.GetOrderByIdAsync(orderId) ?? throw AppException.NotFound("Order not found");

                var orderReadDto = _mapper.Map<OrderReadDto>(foundOrder);
                return orderReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderReadDto> UpdateOrderByIdAsync(Guid orderId, OrderUpdateDto orderUpdateDto)
        {
            var foundOrder = await _orderRepo.GetOrderByIdAsync(orderId);

            if (orderId == Guid.Empty)
            {
                throw AppException.BadRequest("Order id is required");
            }
            if (foundOrder is null)
            {
                throw AppException.NotFound($"Order not found");
            }

            // Update order status and date
            foundOrder.Status = orderUpdateDto.Status;
            foundOrder.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            // Save changes
            var updatedOrder = await _orderRepo.UpdateOrderByIdAsync(foundOrder);

            var orderReadDto = _mapper.Map<OrderReadDto>(updatedOrder);
            orderReadDto.Status = orderUpdateDto.Status;

            return orderReadDto;
        }
    }
}