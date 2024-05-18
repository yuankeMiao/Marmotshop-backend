
using AutoMapper;

using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

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

        public async Task<QueryResult<OrderReadDto>> GetAllOrdersAsync(OrderQueryOptions? options)
        {
            var queryResult = await _orderRepo.GetAllOrdersAsync(options);

            var orders = queryResult.Data;
            var totalCount = queryResult.TotalCount;
            var orderReadDtos = _mapper.Map<IEnumerable<OrderReadDto>>(orders);

            return new QueryResult<OrderReadDto> { Data = orderReadDtos, TotalCount = totalCount };
        }

        public async Task<QueryResult<OrderReadDto>> GetAllOrdersByUserIdAsync(Guid userId, OrderQueryOptions? options)
        {
            // check if user exists
            _ = await _userRepo.GetUserByIdAsync(userId) ?? throw AppException.NotFound("User not found");

            var queryResult = await _orderRepo.GetAllOrdersByUserIdAsync(userId, options);

            var orders = queryResult.Data;
            var totalCount = queryResult.TotalCount;
            var orderReadDtos = _mapper.Map<IEnumerable<OrderReadDto>>(orders);

            return new QueryResult<OrderReadDto> { Data = orderReadDtos, TotalCount = totalCount };
        }

        public async Task<OrderReadDto> GetOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                AppException.InvalidInput("OrderId is required");
            }
            var foundOrder = await _orderRepo.GetOrderByIdAsync(orderId) ?? throw AppException.NotFound("Order not found");
            var orderReadDto = _mapper.Map<OrderReadDto>(foundOrder);

            return orderReadDto;

        }

        public async Task<OrderReadDto> CreateOrderWithTransactionAsync(Guid userId, OrderCreateDto orderCreateDto)
        {
            if (string.IsNullOrWhiteSpace(orderCreateDto.ShippingAddress))
            {
                throw AppException.InvalidInput("Shipping address is required");
            }

            if (orderCreateDto.Products == null || orderCreateDto.Products.Count == 0)
            {
                throw AppException.InvalidInput("At least one product is required to create an order");
            }

            // Check if user exists
            _ = await _userRepo.GetUserByIdAsync(userId) ?? throw AppException.NotFound("User not found");

            // Create order object and assign userId
            var order = _mapper.Map<Order>(orderCreateDto);
            order.UserId = userId;

            // Create orderProducts set
            var newOrderProducts = new HashSet<OrderProduct>();

            foreach (var orderProductDto in orderCreateDto.Products)
            {
                // Check every product in orderCreateDto.OrderProducts
                if(orderProductDto.Quantity < 1) throw AppException.InvalidInput("product quantity should be at least 1");
                var foundProduct = await _productRepo.GetProductByIdAsync(orderProductDto.ProductId);

                // Create a new OrderProduct object
                var newOrderProduct = _mapper.Map<OrderProduct>(foundProduct);
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


        public async Task<OrderReadDto> UpdateOrderByIdAsync(Guid orderId, OrderUpdateDto orderUpdateDto)
        {
            if (orderId == Guid.Empty)
            {
                throw AppException.InvalidInput("Order id is required");
            }

            if (orderUpdateDto == null)
            {
                throw AppException.InvalidInput("Order update data is required");
            }

            var foundOrder = await _orderRepo.GetOrderByIdAsync(orderId);

            // Update order status and date
            foundOrder.Status = orderUpdateDto.Status;
            foundOrder.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            // Save changes
            var updatedOrder = await _orderRepo.UpdateOrderByIdAsync(foundOrder);

            var orderReadDto = _mapper.Map<OrderReadDto>(updatedOrder);
            orderReadDto.Status = orderUpdateDto.Status;

            return orderReadDto;
        }


        public async Task<bool> DeleteOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty) AppException.InvalidInput("Order id is required");

            return await _orderRepo.DeleteOrderByIdAsync(orderId);
        }

    }
}