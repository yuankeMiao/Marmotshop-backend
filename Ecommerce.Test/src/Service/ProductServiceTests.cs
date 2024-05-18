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

namespace Ecommerce.Service.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepo> _mockProductRepo;
        private readonly Mock<ICategoryRepo> _mockCategoryRepo;
        private readonly Mock<IImageRepo> _mockImageRepo;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;
        private readonly List<Category> _categories;
        private readonly List<Product> _products;

        public ProductServiceTests()
        {
            _mockProductRepo = new Mock<IProductRepo>();
            _mockCategoryRepo = new Mock<ICategoryRepo>();
            _mockImageRepo = new Mock<IImageRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });

            _mapper = config.CreateMapper();
            _productService = new ProductService(_mockProductRepo.Object, _mapper, _mockCategoryRepo.Object, _mockImageRepo.Object);
            _categories = SeedingData.GetCategories();
            _products = SeedingData.GetProducts(_categories);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProductReadDtos()
        {
            // Arrange
            var productQueryOptions = new ProductQueryOptions();
            var queryResult = new QueryResult<Product> { Data = _products, TotalCount = _products.Count };
            var productReadDtos = _mapper.Map<List<ProductReadDto>>(_products);

            _mockProductRepo.Setup(repo => repo.GetAllProductsAsync(productQueryOptions)).ReturnsAsync(queryResult);

            // Act
            var result = await _productService.GetAllProductsAsync(productQueryOptions);

            // Assert
            Assert.Equal(productReadDtos.Count, result.Data.Count());
            Assert.Equal(_products.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetProductByIdAsync_ValidId_ReturnsProductReadDto()
        {
            // Arrange
            var productId = _products[0].Id;
            var productReadDto = _mapper.Map<ProductReadDto>(_products[0]);

            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(_products[0]);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            AssertProductReadDtoEqual(productReadDto, result);
        }

        [Fact]
        public async Task GetProductByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(productId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _productService.GetProductByIdAsync(productId));
        }

        [Fact]
        public async Task CreateProductAsync_ValidData_ReturnsProductReadDto()
        {
            // Arrange
            var categoryId = _categories[0].Id;
            var productCreateDto = new ProductCreateDto
            {
                Title = "New Product",
                Description = "Description of the new product",
                CategoryId = categoryId,
                Price = 100,
                DiscountPercentage = 10,
                Stock = 50,
                Thumbnail = "http://example.com/image.jpg",
                Images = new List<ImageCreateDto>
                {
                    new ImageCreateDto { Url = "http://example.com/image1.jpg", ProductId = Guid.NewGuid() },
                    new ImageCreateDto { Url = "http://example.com/image2.jpg", ProductId = Guid.NewGuid() }
                }
            };
            var newProduct = _mapper.Map<Product>(productCreateDto);
            var productReadDto = _mapper.Map<ProductReadDto>(newProduct);

            _mockCategoryRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(_categories[0]);
            _mockProductRepo.Setup(repo => repo.CreateProductWithTransactionAsync(It.IsAny<Product>())).ReturnsAsync(newProduct);

            // Act
            var result = await _productService.CreateProductAsync(productCreateDto);

            // Assert
            AssertProductReadDtoEqual(productReadDto, result);
        }

        [Theory]
        [InlineData("", "Description", 100, 10, 50, "http://example.com/image.jpg")]
        [InlineData("Title", "", 100, 10, 50, "http://example.com/image.jpg")]
        [InlineData("Title", "Description", -10, 10, 50, "http://example.com/image.jpg")]
        [InlineData("Title", "Description", 100, -5, 50, "http://example.com/image.jpg")]
        [InlineData("Title", "Description", 100, 10, -5, "http://example.com/image.jpg")]
        [InlineData("Title", "Description", 100, 10, 50, "")]
        [InlineData("Title", "Description", 100, 10, 50, "invalidUrl")]
        public async Task CreateProductAsync_InvalidData_ThrowsException(string title, string description, decimal price, int discountPercentage, int stock, string thumbnail)
        {
            // Arrange
            var categoryId = _categories[0].Id;
            var invalidProductCreateDto = new ProductCreateDto
            {
                Title = title,
                Description = description,
                CategoryId = categoryId,
                Price = price,
                DiscountPercentage = discountPercentage,
                Stock = stock,
                Thumbnail = thumbnail,
                Images = new List<ImageCreateDto>
                {
                    new ImageCreateDto { Url = "http://example.com/image1.jpg", ProductId = Guid.NewGuid() },
                    new ImageCreateDto { Url = "http://example.com/image2.jpg", ProductId = Guid.NewGuid() }
                }
            };

            _mockCategoryRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(_categories[0]);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _productService.CreateProductAsync(invalidProductCreateDto));
        }

        [Fact]
        public async Task UpdateProductByIdAsync_ValidData_ReturnsUpdatedProductReadDto()
        {
            // Arrange
            var productId = _products[0].Id;
            var productUpdateDto = new ProductUpdateDto { Title = "Updated Product", Price = 200 };
            var updatedProduct = _products[0];
            updatedProduct.Title = productUpdateDto.Title;
            updatedProduct.Price = productUpdateDto.Price.Value;
            updatedProduct.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            var productReadDto = _mapper.Map<ProductReadDto>(updatedProduct);

            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(_products[0]);
            _mockProductRepo.Setup(repo => repo.UpdateProductByIdAsync(It.IsAny<Product>())).ReturnsAsync(updatedProduct);

            // Act
            var result = await _productService.UpdateProductByIdAsync(productId, productUpdateDto);

            // Assert
            AssertProductReadDtoEqual(productReadDto, result);
        }

        [Theory]
        [InlineData("", "Updated Description", 200, 10, 50, "http://example.com/image.jpg")]
        [InlineData("Updated Title", "", 200, 10, 50, "http://example.com/image.jpg")]
        [InlineData("Updated Title", "Updated Description", -10, 10, 50, "http://example.com/image.jpg")]
        [InlineData("Updated Title", "Updated Description", 200, -5, 50, "http://example.com/image.jpg")]
        [InlineData("Updated Title", "Updated Description", 200, 10, -5, "http://example.com/image.jpg")]
        [InlineData("Updated Title", "Updated Description", 200, 10, 50, "")]
        [InlineData("Updated Title", "Updated Description", 200, 10, 50, "invalidUrl")]
        public async Task UpdateProductByIdAsync_InvalidData_ThrowsException(string title, string description, decimal price, int discountPercentage, int stock, string thumbnail)
        {
            // Arrange
            var productId = _products[0].Id;
            var invalidProductUpdateDto = new ProductUpdateDto
            {
                Title = title,
                Description = description,
                Price = price,
                DiscountPercentage = discountPercentage,
                Stock = stock,
                Thumbnail = thumbnail,
                Images = new List<ImageUpdateDto>
                {
                    new ImageUpdateDto { Url = "http://example.com/image1.jpg" },
                    new ImageUpdateDto { Url = "http://example.com/image2.jpg" }
                }
            };

            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(_products[0]);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _productService.UpdateProductByIdAsync(productId, invalidProductUpdateDto));
        }

        [Fact]
        public async Task DeleteProductByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var productId = _products[0].Id;

            _mockProductRepo.Setup(repo => repo.DeleteProductByIdAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProductByIdAsync(productId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _mockProductRepo.Setup(repo => repo.DeleteProductByIdAsync(productId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _productService.DeleteProductByIdAsync(productId));
        }

        // i use this helper method to compare ProductReadDto
        private void AssertProductReadDtoEqual(ProductReadDto expected, ProductReadDto actual)
        {
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.CategoryId, actual.CategoryId);
            Assert.Equal(expected.Price, actual.Price);
            Assert.Equal(expected.DiscountPercentage, actual.DiscountPercentage);
            Assert.Equal(expected.Stock, actual.Stock);
            Assert.Equal(expected.Thumbnail, actual.Thumbnail);
            Assert.Equal(expected.CreatedDate, actual.CreatedDate);
            Assert.Equal(expected.UpdatedDate, actual.UpdatedDate);
        }
    }
}
