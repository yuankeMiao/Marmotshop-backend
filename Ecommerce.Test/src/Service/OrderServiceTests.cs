using Xunit;
using Moq;
using AutoMapper;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.DTO;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.Entity;
using Ecommerce.Service.src.Shared;
using Ecommerce.WebAPI.src.Database;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepo> _mockOrderRepo;
        private readonly Mock<IProductRepo> _mockProductRepo;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;
        private readonly List<Order> _orders;
        private readonly List<Product> _products;
        private readonly List<User> _users;
        private readonly List<Category> _categories;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IOrderRepo>();
            _mockProductRepo = new Mock<IProductRepo>();
            _mockUserRepo = new Mock<IUserRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });

            _mapper = config.CreateMapper();
            _orderService = new OrderService(_mockOrderRepo.Object, _mapper, _mockProductRepo.Object, _mockUserRepo.Object);
            _users = SeedingData.GetUsers();
            _categories = SeedingData.GetCategories();
            _orders = SeedingData.GetOrders(_users);
            _products = SeedingData.GetProducts(_categories);

        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsOrderReadDtos()
        {
            // Arrange
            var orderQueryOptions = new OrderQueryOptions();
            var queryResult = new QueryResult<Order> { Data = _orders, TotalCount = _orders.Count };
            var orderReadDtos = _mapper.Map<List<OrderReadDto>>(_orders);

            _mockOrderRepo.Setup(repo => repo.GetAllOrdersAsync(orderQueryOptions)).ReturnsAsync(queryResult);

            // Act
            var result = await _orderService.GetAllOrdersAsync(orderQueryOptions);

            // Assert
            AssertCollectionsEqual(orderReadDtos, result.Data);
            Assert.Equal(_orders.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetAllOrdersByUserIdAsync_ValidUserId_ReturnsOrderReadDtos()
        {
            // Arrange
            var userId = _users[0].Id;
            var orderQueryOptions = new OrderQueryOptions();
            var orders = _orders.Where(o => o.UserId == userId).ToList();
            var queryResult = new QueryResult<Order> { Data = orders, TotalCount = orders.Count };
            var orderReadDtos = _mapper.Map<List<OrderReadDto>>(orders);

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockOrderRepo.Setup(repo => repo.GetAllOrdersByUserIdAsync(userId, orderQueryOptions)).ReturnsAsync(queryResult);

            // Act
            var result = await _orderService.GetAllOrdersByUserIdAsync(userId, orderQueryOptions);

            // Assert
            AssertCollectionsEqual(orderReadDtos, result.Data);
            Assert.Equal(orders.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetAllOrdersByUserIdAsync_InvalidUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderQueryOptions = new OrderQueryOptions();

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _orderService.GetAllOrdersByUserIdAsync(userId, orderQueryOptions));
        }

        [Fact]
        public async Task GetOrderByIdAsync_ValidId_ReturnsOrderReadDto()
        {
            // Arrange
            var orderId = _orders[0].Id;
            var orderReadDto = _mapper.Map<OrderReadDto>(_orders[0]);

            _mockOrderRepo.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(_orders[0]);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            AssertOrderReadDtoEqual(orderReadDto, result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mockOrderRepo.Setup(repo => repo.GetOrderByIdAsync(orderId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _orderService.GetOrderByIdAsync(orderId));
        }

        [Fact]
        public async Task CreateOrderWithTransactionAsync_ValidData_ReturnsOrderReadDto()
        {
            // Arrange
            var userId = _users[0].Id;
            var orderCreateDto = new OrderCreateDto
            {
                ShippingAddress = "123 Main St",
                Products = new HashSet<OrderProductCreateDto>
                {
                    new OrderProductCreateDto { ProductId = _products[0].Id, Quantity = 2 }
                }
            };

            var newOrderProducts = new HashSet<OrderProduct>
            {
                new OrderProduct
                {
                    Id = Guid.NewGuid(),
                    ProductId = _products[0].Id,
                    Title = _products[0].Title,
                    Thumbnail = _products[0].Thumbnail,
                    ActualPrice = _products[0].Price,
                    Quantity = 2,
                    TotalPrice = 2 * _products[0].Price
                }
            };

            var createdOrder = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShippingAddress = orderCreateDto.ShippingAddress,
                Products = newOrderProducts,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var orderReadDto = _mapper.Map<OrderReadDto>(createdOrder);

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(_products[0].Id)).ReturnsAsync(_products[0]);
            _mockOrderRepo.Setup(repo => repo.CreateOrderWithTransactionAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);

            // Act
            var result = await _orderService.CreateOrderWithTransactionAsync(userId, orderCreateDto);

            // Assert
            AssertOrderReadDtoEqual(orderReadDto, result);
        }

        [Fact]
        public async Task CreateOrderWithTransactionAsync_InvalidData_ThrowsException()
        {
            // Arrange
            var userId = _users[0].Id;
            var invalidProductId = Guid.NewGuid();
            var orderCreateDto = new OrderCreateDto
            {
                ShippingAddress = "",
                Products =
        [
            new OrderProductCreateDto
            {
                ProductId = invalidProductId,
                Quantity = 2
            }
        ]
            };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(invalidProductId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _orderService.CreateOrderWithTransactionAsync(userId, orderCreateDto));
        }


        [Fact]
        public async Task UpdateOrderByIdAsync_ValidData_ReturnsUpdatedOrderReadDto()
        {
            // Arrange
            var orderId = _orders[0].Id;
            var orderUpdateDto = new OrderUpdateDto { Status = OrderStatus.Completed };
            var updatedOrder = _orders[0];
            updatedOrder.Status = orderUpdateDto.Status;
            updatedOrder.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            var orderReadDto = _mapper.Map<OrderReadDto>(updatedOrder);

            _mockOrderRepo.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(_orders[0]);
            _mockOrderRepo.Setup(repo => repo.UpdateOrderByIdAsync(It.IsAny<Order>())).ReturnsAsync(updatedOrder);

            // Act
            var result = await _orderService.UpdateOrderByIdAsync(orderId, orderUpdateDto);

            // Assert
            AssertOrderReadDtoEqual(orderReadDto, result);
        }

        [Fact]
        public async Task DeleteOrderByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var orderId = _orders[0].Id;

            _mockOrderRepo.Setup(repo => repo.DeleteOrderByIdAsync(orderId)).ReturnsAsync(true);

            // Act
            var result = await _orderService.DeleteOrderByIdAsync(orderId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOrderByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mockOrderRepo.Setup(repo => repo.DeleteOrderByIdAsync(orderId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _orderService.DeleteOrderByIdAsync(orderId));
        }

        private void AssertCollectionsEqual(IEnumerable<OrderReadDto> expected, IEnumerable<OrderReadDto> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                AssertOrderReadDtoEqual(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        private void AssertOrderReadDtoEqual(OrderReadDto expected, OrderReadDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.ShippingAddress, actual.ShippingAddress);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.CreatedDate, actual.CreatedDate);
            Assert.Equal(expected.UpdatedDate, actual.UpdatedDate);
            AssertCollectionsEqual(expected.Products, actual.Products);
        }

        private void AssertCollectionsEqual(IEnumerable<OrderProductReadDto> expected, IEnumerable<OrderProductReadDto> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                AssertOrderProductReadDtoEqual(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        private void AssertOrderProductReadDtoEqual(OrderProductReadDto expected, OrderProductReadDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Thumbnail, actual.Thumbnail);
            Assert.Equal(expected.ActualPrice, actual.ActualPrice);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.TotalPrice, actual.TotalPrice);
        }
    }
}
