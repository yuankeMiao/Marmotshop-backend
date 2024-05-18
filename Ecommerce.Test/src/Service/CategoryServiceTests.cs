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
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepo> _mockCategoryRepo;
        private readonly IMapper _mapper;
        private readonly CategoryService _categoryService;
        private readonly List<Category> _categories;

        public CategoryServiceTests()
        {
            _mockCategoryRepo = new Mock<ICategoryRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });

            _mapper = config.CreateMapper();
            _categoryService = new CategoryService(_mapper, _mockCategoryRepo.Object);
            _categories = SeedingData.GetCategories();
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsCategoryReadDtos()
        {
            // Arrange
            _mockCategoryRepo.Setup(repo => repo.GetAllCategoriesAsync()).ReturnsAsync(_categories);
            var expectedDtos = _mapper.Map<List<CategoryReadDto>>(_categories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();
            var resultList = result.ToList();

            // Assert
            Assert.Equal(expectedDtos.Count, resultList.Count);
            for (int i = 0; i < expectedDtos.Count; i++)
            {
                AssertCategoryReadDtoEqual(expectedDtos[i], resultList[i]);
            }
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsCategoryReadDto()
        {
            // Arrange
            var categoryId = _categories[0].Id;
            var expectedDto = _mapper.Map<CategoryReadDto>(_categories[0]);

            _mockCategoryRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(_categories[0]);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            AssertCategoryReadDtoEqual(expectedDto, result);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _mockCategoryRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _categoryService.GetCategoryByIdAsync(categoryId));
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidData_ReturnsCategoryReadDto()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreateDto
            {
                Name = "New Category",
                Image = "http://example.com/image.jpg"
            };
            var newCategory = _mapper.Map<Category>(categoryCreateDto);
            var createdCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = newCategory.Name,
                Image = newCategory.Image,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var expectedDto = _mapper.Map<CategoryReadDto>(createdCategory);

            _mockCategoryRepo.Setup(repo => repo.CreateCategoryAsync(It.IsAny<Category>())).ReturnsAsync(createdCategory);

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryCreateDto);

            // Assert
            AssertCategoryReadDtoEqual(expectedDto, result);
        }

        [Theory]
        [InlineData("", "http://example.com/image.jpg")]
        [InlineData("Valid Name", "invalidUrl")]
        public async Task CreateCategoryAsync_InvalidData_ThrowsException(string name, string image)
        {
            // Arrange
            var invalidCategoryCreateDto = new CategoryCreateDto
            {
                Name = name,
                Image = image
            };

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _categoryService.CreateCategoryAsync(invalidCategoryCreateDto));
        }

        [Fact]
        public async Task UpdateCategoryByIdAsync_ValidData_ReturnsUpdatedCategoryReadDto()
        {
            // Arrange
            var categoryId = _categories[0].Id;
            var categoryUpdateDto = new CategoryUpdateDto { Name = "Updated Category", Image = "http://example.com/newimage.jpg" };
            var updatedCategory = _categories[0];
            updatedCategory.Name = categoryUpdateDto.Name;
            updatedCategory.Image = categoryUpdateDto.Image;
            updatedCategory.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            var expectedDto = _mapper.Map<CategoryReadDto>(updatedCategory);

            _mockCategoryRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(_categories[0]);
            _mockCategoryRepo.Setup(repo => repo.UpdateCategoryByIdAsync(It.IsAny<Category>())).ReturnsAsync(updatedCategory);

            // Act
            var result = await _categoryService.UpdateCategoryByIdAsync(categoryId, categoryUpdateDto);

            // Assert
            AssertCategoryReadDtoEqual(expectedDto, result);
        }

        [Theory]
        [InlineData("", "http://example.com/image.jpg")]
        [InlineData("Updated Category", "invalidUrl")]
        public async Task UpdateCategoryByIdAsync_InvalidData_ThrowsException(string name, string image)
        {
            // Arrange
            var categoryId = _categories[0].Id;
            var invalidCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = name,
                Image = image
            };

            _mockCategoryRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(_categories[0]);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _categoryService.UpdateCategoryByIdAsync(categoryId, invalidCategoryUpdateDto));
        }

        [Fact]
        public async Task DeleteCategoryByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var categoryId = _categories[0].Id;

            _mockCategoryRepo.Setup(repo => repo.DeleteCategoryByIdAsync(categoryId)).ReturnsAsync(true);

            // Act
            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCategoryByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _mockCategoryRepo.Setup(repo => repo.DeleteCategoryByIdAsync(categoryId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _categoryService.DeleteCategoryByIdAsync(categoryId));
        }

        // a helper method to compare CategoryReadDto
        private void AssertCategoryReadDtoEqual(CategoryReadDto expected, CategoryReadDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Image, actual.Image);
            Assert.Equal(expected.CreatedDate, actual.CreatedDate);
            Assert.Equal(expected.UpdatedDate, actual.UpdatedDate);
        }
    }
}
