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

        public OrderService(IOrderRepo orderRepo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
        }

        public async Task<OrderReadDto> CreateOrderWithtransactionAsync(Guid userId, OrderCreateDto orderCreateDto)
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
                var foundProduct = await _productRepo.GetProductByIdAsync(orderProductDto.ProductId);
                // if found, create a new OrderProduct object
                var newOrderProduct = _mapper.Map<Product, OrderProduct>(foundProduct);
                newOrderProduct.Quantity = orderProductDto.Quantity;
                newOrderProduct.TotalPrice = newOrderProduct.Quantity * newOrderProduct.ActualPrice;
                newOrderProduct.OrderId = order.Id;

                newOrderProducts.Add(newOrderProduct);
            }

            order.Products = newOrderProducts;
            order.Status = OrderStatus.Pending;
            order.ShippingAddress = orderCreateDto.ShippingAddress;

            var createdOrder = await _orderRepo.CreateOrderWithTransactionAsync(order);
            var orderReadDto = _mapper.Map<OrderReadDto>(createdOrder);
            return orderReadDto;
        }

        public async Task<bool> DeleteOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                AppException.InvalidInput("OrderId is required");
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
            try
            {
                var orders = await _orderRepo.GetAllOrdersAsync(options);
                var orderReadDtos = orders.Select(o => _mapper.Map<Order, OrderReadDto>(o));

                return orderReadDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderReadDto> GetOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                AppException.InvalidInput("OrderId is required");
            }
            try
            {
                var foundOrder = await _orderRepo.GetOrderByIdAsync(orderId) ?? throw AppException.NotFound("Order not found");
                var orderReadDto = _mapper.Map<OrderReadDto>(foundOrder);
                
                // automapper supports nested mapping, so i dont need it, will try it later
                // orderReadDto.Products = _mapper.Map<IEnumerable<OrderProduct>, HashSet<OrderProductReadDto>>(foundOrder.Products);

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
                throw AppException.InvalidInput("Order id is required");
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