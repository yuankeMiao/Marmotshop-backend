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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Service.Tests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepo> _mockReviewRepo;
        private readonly Mock<IProductRepo> _mockProductRepo;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<IOrderRepo> _mockOrderRepo;
        private readonly IMapper _mapper;
        private readonly ReviewService _reviewService;
        private readonly List<Review> _reviews;
        private readonly List<Product> _products;
        private readonly List<User> _users;
        private readonly List<Order> _orders;
        private readonly List<Category> _categories;
        private readonly List<OrderProduct> _orderProducts;

        public ReviewServiceTests()
        {
            _mockReviewRepo = new Mock<IReviewRepo>();
            _mockProductRepo = new Mock<IProductRepo>();
            _mockUserRepo = new Mock<IUserRepo>();
            _mockOrderRepo = new Mock<IOrderRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });

            _mapper = config.CreateMapper();
            _reviewService = new ReviewService(_mockReviewRepo.Object, _mapper, _mockProductRepo.Object, _mockUserRepo.Object, _mockOrderRepo.Object);
            _users = SeedingData.GetUsers();
            _categories = SeedingData.GetCategories();
            _products = SeedingData.GetProducts(_categories);
            _reviews = SeedingData.GetReviews(_users, _products);
            _orders = SeedingData.GetOrders(_users);
            _orderProducts = SeedingData.GetOrderProducts(_orders, _products);
        }

        [Fact]
        public async Task GetAllReviewsAsync_ReturnsReviewReadDtos()
        {
            // Arrange
            var reviewQueryOptions = new ReviewQueryOptions();
            var queryResult = new QueryResult<Review> { Data = _reviews, TotalCount = _reviews.Count };
            var reviewReadDtos = _mapper.Map<List<ReviewReadDto>>(_reviews);

            _mockReviewRepo.Setup(repo => repo.GetAllReviewsAsync(reviewQueryOptions)).ReturnsAsync(queryResult);

            // Act
            var result = await _reviewService.GetAllReviewsAsync(reviewQueryOptions);

            // Assert
            for (int i = 0; i < reviewReadDtos.Count; i++)
            {
                AssertReviewReadDtoEqual(reviewReadDtos[i], result.Data.ToList()[i]);
            }
            Assert.Equal(_reviews.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetAllReviewsByProductIdAsync_ValidProductId_ReturnsReviewReadDtos()
        {
            // Arrange
            var productId = _products[0].Id;
            var reviewQueryOptions = new ReviewQueryOptions();
            var reviews = _reviews.Where(r => r.ProductId == productId).ToList();
            var queryResult = new QueryResult<Review> { Data = reviews, TotalCount = reviews.Count };
            var reviewReadDtos = _mapper.Map<List<ReviewReadDto>>(reviews);

            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(_products[0]);
            _mockReviewRepo.Setup(repo => repo.GetAllReviewsByProductIdAsync(productId, reviewQueryOptions)).ReturnsAsync(queryResult);

            // Act
            var result = await _reviewService.GetAllReviewsByProductIdAsync(productId, reviewQueryOptions);

            // Assert
            for (int i = 0; i < reviewReadDtos.Count; i++)
            {
                AssertReviewReadDtoEqual(reviewReadDtos[i], result.Data.ToList()[i]);
            }
            Assert.Equal(reviews.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetAllReviewsByProductIdAsync_InvalidProductId_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var reviewQueryOptions = new ReviewQueryOptions();

            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(productId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _reviewService.GetAllReviewsByProductIdAsync(productId, reviewQueryOptions));
        }

        [Fact]
        public async Task GetAllReviewsByUserIdAsync_ValidUserId_ReturnsReviewReadDtos()
        {
            // Arrange
            var userId = _users[0].Id;
            var reviewQueryOptions = new ReviewQueryOptions();
            var reviews = _reviews.Where(r => r.UserId == userId).ToList();
            var queryResult = new QueryResult<Review> { Data = reviews, TotalCount = reviews.Count };
            var reviewReadDtos = _mapper.Map<List<ReviewReadDto>>(reviews);

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockReviewRepo.Setup(repo => repo.GetAllReviewsByUserIdAsync(userId, reviewQueryOptions)).ReturnsAsync(queryResult);

            // Act
            var result = await _reviewService.GetAllReviewsByUserIdAsync(userId, reviewQueryOptions);

            // Assert
            for (int i = 0; i < reviewReadDtos.Count; i++)
            {
                AssertReviewReadDtoEqual(reviewReadDtos[i], result.Data.ToList()[i]);
            }
            Assert.Equal(reviews.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetAllReviewsByUserIdAsync_InvalidUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var reviewQueryOptions = new ReviewQueryOptions();

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _reviewService.GetAllReviewsByUserIdAsync(userId, reviewQueryOptions));
        }

        [Fact]
        public async Task GetReviewByIdAsync_ValidId_ReturnsReviewReadDto()
        {
            // Arrange
            var reviewId = _reviews[0].Id;
            var reviewReadDto = _mapper.Map<ReviewReadDto>(_reviews[0]);

            _mockReviewRepo.Setup(repo => repo.GetReviewByIdAsync(reviewId)).ReturnsAsync(_reviews[0]);

            // Act
            var result = await _reviewService.GetReviewByIdAsync(reviewId);

            // Assert
            AssertReviewReadDtoEqual(reviewReadDto, result);
        }

        [Fact]
        public async Task GetReviewByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var reviewId = Guid.NewGuid();

            _mockReviewRepo.Setup(repo => repo.GetReviewByIdAsync(reviewId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _reviewService.GetReviewByIdAsync(reviewId));
        }


        [Fact]
        public async Task CreateReviewAsync_ValidData_ReturnsReviewReadDto()
        {
            // Arrange
            var validOrder = _orders[0];
            var validOrderProducts = _orderProducts.Where(op => op.OrderId == validOrder.Id);
            validOrder.Products = validOrderProducts.ToHashSet();

            var validProductId = validOrderProducts.ToList()[0].ProductId;
            var validProduct = _products.FirstOrDefault(p => p.Id == validProductId)!;

            var validUserId = validOrder.UserId;
            var validUser = _users.FirstOrDefault(u => u.Id == validUserId)!;
            var validOrders = _orders.Where(op => op.UserId == validUserId)!;

            var reviewCreateDto = new ReviewCreateDto
            {
                Rating = 5,
                Content = "Great product!",
                ProductId = validProductId
            };

            var createdReview = new Review
            {
                Id = Guid.NewGuid(),
                UserId = validUserId,
                ProductId = validProductId,
                Rating = reviewCreateDto.Rating,
                Content = reviewCreateDto.Content,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var reviewReadDto = _mapper.Map<ReviewReadDto>(createdReview);

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(validUser);
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(validProduct);

            _mockOrderRepo.Setup(repo => repo.GetAllOrdersByUserIdAsync(It.IsAny<Guid>(), null))
                          .ReturnsAsync(new QueryResult<Order> { Data = validOrders, TotalCount = validOrders.Count() });
            _mockReviewRepo.Setup(repo => repo.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            // Act
            var result = await _reviewService.CreateReviewAsync(validUserId, reviewCreateDto);

            // Assert
            AssertReviewReadDtoEqual(reviewReadDto, result);
        }


        [Fact]
        public async Task CreateReviewAsync_InvalidData_ThrowsException()
        {
            // Arrange
            var userId = _users[0].Id;
            var reviewCreateDto = new ReviewCreateDto
            {
                Rating = 5,
                Content = "Great product!",
                ProductId = Guid.NewGuid() // invalid product ID
            };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(reviewCreateDto.ProductId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _reviewService.CreateReviewAsync(userId, reviewCreateDto));
        }

        [Fact]
        public async Task UpdateReviewByIdAsync_ValidData_ReturnsUpdatedReviewReadDto()
        {
            // Arrange
            var reviewId = _reviews[0].Id;
            var reviewUpdateDto = new ReviewUpdateDto { Rating = 4, Content = "Updated review content" };
            var updatedReview = _reviews[0];
            updatedReview.Rating = reviewUpdateDto.Rating ?? updatedReview.Rating;
            updatedReview.Content = reviewUpdateDto.Content ?? updatedReview.Content;
            updatedReview.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            var reviewReadDto = _mapper.Map<ReviewReadDto>(updatedReview);

            _mockReviewRepo.Setup(repo => repo.GetReviewByIdAsync(reviewId)).ReturnsAsync(_reviews[0]);
            _mockReviewRepo.Setup(repo => repo.UpdateReviewByIdAsync(It.IsAny<Review>())).ReturnsAsync(updatedReview);

            // Act
            var result = await _reviewService.UpdateReviewByIdAsync(reviewId, reviewUpdateDto);

            // Assert
            AssertReviewReadDtoEqual(reviewReadDto, result);
        }

        [Fact]
        public async Task DeleteReviewByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var reviewId = _reviews[0].Id;

            _mockReviewRepo.Setup(repo => repo.DeleteReviewByIdAsync(reviewId)).ReturnsAsync(true);

            // Act
            var result = await _reviewService.DeleteReviewByIdAsync(reviewId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteReviewByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var reviewId = Guid.NewGuid();

            _mockReviewRepo.Setup(repo => repo.DeleteReviewByIdAsync(reviewId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _reviewService.DeleteReviewByIdAsync(reviewId));
        }

        private void AssertReviewReadDtoEqual(ReviewReadDto expected, ReviewReadDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Rating, actual.Rating);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.CreatedDate, actual.CreatedDate);
            Assert.Equal(expected.UpdatedDate, actual.UpdatedDate);
        }
    }
}
