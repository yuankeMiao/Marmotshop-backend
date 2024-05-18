using Xunit;
using Moq;
using AutoMapper;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.Entity;
using Ecommerce.WebAPI.src.Database;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.Shared;

namespace Ecommerce.Service.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly IMapper _mapper;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly UserService _userService;
        private readonly List<User> _users;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _mockPasswordService = new Mock<IPasswordService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });

            _mapper = config.CreateMapper();
            _userService = new UserService(_mapper, _mockUserRepo.Object, _mockPasswordService.Object);
            _users = SeedingData.GetUsers();
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsQueryResult()
        {
            // Arrange
            var options = new UserQueryOptions();
            var queryResult = new QueryResult<User> { Data = _users, TotalCount = _users.Count };
            var userReadDtos = _mapper.Map<List<UserReadDto>>(_users);

            _mockUserRepo.Setup(repo => repo.GetAllUsersAsync(options)).ReturnsAsync(queryResult);

            // Act
            var result = await _userService.GetAllUsersAsync(options);

            // Assert
            Assert.Equal(_users.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidId_ReturnsUserReadDto()
        {
            // Arrange
            var userId = _users[0].Id;
            var userReadDto = _mapper.Map<UserReadDto>(_users[0]);

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            AssertUserReadDtoEqual(userReadDto, result);
        }

        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.GetUserByIdAsync(userId));
        }

        [Fact]
        public async Task GetUserByEmailAsync_ValidEmail_ReturnsUserReadDto()
        {
            // Arrange
            var email = _users[0].Email;
            var userReadDto = _mapper.Map<UserReadDto>(_users[0]);

            _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(_users[0]);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            AssertUserReadDtoEqual(userReadDto, result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_InvalidEmail_ThrowsException()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.GetUserByEmailAsync(email));
        }

        [Fact]
        public async Task CreateUserAsync_ValidData_ReturnsUserReadDto()
        {
            // Arrange
            var userCreateDto = new UserCreateDto
            {
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123",
                Avatar = "http://example.com/avatar.jpg",
                Role = UserRole.Customer,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                UpdatedDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var newUser = _mapper.Map<User>(userCreateDto);
            newUser.Password = "hasedPassword";
            var userReadDto = _mapper.Map<UserReadDto>(newUser);

            _mockPasswordService.Setup(p => p.HashPassword(userCreateDto.Password, out It.Ref<byte[]>.IsAny)).Returns("hashedPassword");
            _mockUserRepo.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(newUser);

            // Act
            var result = await _userService.CreateUserAsync(userCreateDto);

            // Assert
            AssertUserReadDtoEqual(userReadDto, result);
        }

        [Theory]
        [InlineData("", "Doe", "john.doe@example.com", "Password123", "http://example.com/avatar.jpg")]
        [InlineData("John", "", "john.doe@example.com", "Password123", "http://example.com/avatar.jpg")]
        [InlineData("John", "Doe", "invalidEmail", "Password123", "http://example.com/avatar.jpg")]
        [InlineData("John", "Doe", "john.doe@example.com", "123", "http://example.com/avatar.jpg")]
        [InlineData("John", "Doe", "john.doe@example.com", "Password123", "invalidUrl")]
        public async Task CreateUserAsync_InvalidData_ThrowsException(string firstname, string lastname, string email, string password, string avatar)
        {
            // Arrange
            var invalidUserCreateDto = new UserCreateDto
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                Password = password,
                Avatar = avatar
            };

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _userService.CreateUserAsync(invalidUserCreateDto));
        }

        [Fact]
        public async Task UpdateUserByIdAsync_ValidData_ReturnsUpdatedUserReadDto()
        {
            // Arrange
            var userId = _users[0].Id;
            var userUpdateDto = new UserUpdateDto { Firstname = "Jane", Lastname = "Doe" };
            var updatedUser = _users[0];
            updatedUser.Firstname = userUpdateDto.Firstname;
            updatedUser.Lastname = userUpdateDto.Lastname;
            var userReadDto = _mapper.Map<UserReadDto>(updatedUser);

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockUserRepo.Setup(repo => repo.UpdateUserByIdAsync(It.IsAny<User>())).ReturnsAsync(updatedUser);

            // Act
            var result = await _userService.UpdateUserByIdAsync(userId, userUpdateDto);

            // Assert
            AssertUserReadDtoEqual(userReadDto, result);
        }

        [Theory]
        [InlineData("", "Doe", "john.doe@example.com", "Password123", "http://example.com/avatar.jpg")]
        [InlineData("John", "", "john.doe@example.com", "Password123", "http://example.com/avatar.jpg")]
        [InlineData("John", "Doe", "invalidEmail", "Password123", "http://example.com/avatar.jpg")]
        [InlineData("John", "Doe", "john.doe@example.com", "123", "http://example.com/avatar.jpg")]
        [InlineData("John", "Doe", "john.doe@example.com", "Password123", "invalidUrl")]
        public async Task UpdateUserByIdAsync_InvalidData_ThrowsException(string firstname, string lastname, string email, string password, string avatar)
        {
            // Arrange
            var userId = _users[0].Id;
            var invalidUserUpdateDto = new UserUpdateDto
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                Password = password,
                Avatar = avatar
            };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _userService.UpdateUserByIdAsync(userId, invalidUserUpdateDto));
        }

        [Fact]
        public async Task DeleteUserByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var userId = _users[0].Id;

            _mockUserRepo.Setup(repo => repo.DeleteUserByIdAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUserByIdAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepo.Setup(repo => repo.DeleteUserByIdAsync(userId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.DeleteUserByIdAsync(userId));
        }

        private void AssertUserReadDtoEqual(UserReadDto expected, UserReadDto actual)
        {
            Assert.Equal(expected.Firstname, actual.Firstname);
            Assert.Equal(expected.Lastname, actual.Lastname);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Avatar, actual.Avatar);
            Assert.Equal(expected.Role, actual.Role);
        }
    }
}
