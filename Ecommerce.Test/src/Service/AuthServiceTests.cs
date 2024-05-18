using Xunit;
using Moq;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.Entity;
using Ecommerce.WebAPI.src.Database;

namespace Ecommerce.Service.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly AuthService _authService;
        private readonly List<User> _users;

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _authService = new AuthService(_mockUserRepo.Object, _mockTokenService.Object, _mockPasswordService.Object);
            _users = SeedingData.GetUsers();
        }

        // Tests for LoginAsync method

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenResponseDto()
        {
            // Arrange
            var mockUser = _users[0];
            mockUser.Password = $"{mockUser.Firstname} {mockUser.Lastname}";
            var userCredential = new UserCredential { Email = mockUser.Email, Password = mockUser.Password };
            var tokenResponse = new TokenResponseDto { AccessToken = "accessToken", RefreshToken = "refreshToken" };

            _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(userCredential.Email)).ReturnsAsync(mockUser);
            _mockPasswordService.Setup(service => service.VerifyPassword(userCredential.Password, mockUser.Password, mockUser.Salt)).Returns(true);
            _mockTokenService.Setup(service => service.GetToken(mockUser)).Returns(tokenResponse);

            // Act
            var result = await _authService.LoginAsync(userCredential);

            // Assert
            Assert.Equal(tokenResponse.AccessToken, result.AccessToken);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var userCredential = new UserCredential { Email = "nonexistent@example.com", Password = "password" };
            _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(userCredential.Email)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _authService.LoginAsync(userCredential));
        }

        // Tests for RefreshTokenAsync method

        [Fact]
        public async Task RefreshTokenAsync_ValidData_ReturnsTokenResponseDto()
        {
            // Arrange
            var refreshToken = "refresh_token";
            var tokenResponse = new TokenResponseDto { AccessToken = "new_accessToken", RefreshToken = "new_refreshToken" };

            var userId = _users[0].Id;
            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockTokenService.Setup(service => service.RefreshToken(refreshToken, _users[0])).Returns(tokenResponse);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken, userId);

            // Assert
            Assert.Equal(tokenResponse.AccessToken, result.AccessToken);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var refreshToken = "refresh_token";
            var userId = Guid.NewGuid();
            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _authService.RefreshTokenAsync(refreshToken, userId));
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidRefreshToken_ThrowsException()
        {
            // Arrange
            var refreshToken = "invalid_refresh_token";
            var userId = _users[0].Id;
            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(_users[0]);
            _mockTokenService.Setup(service => service.RefreshToken(refreshToken, _users[0])).Throws<SecurityTokenException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.RefreshTokenAsync(refreshToken, userId));
        }

        // Tests for LogoutAsync method

        [Fact]
        public async Task LogoutAsync_ValidUserId_ReturnsTrue()
        {
            // Arrange
            var userId = _users[0].Id;
            _mockTokenService.Setup(service => service.InvalidateTokenAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LogoutAsync_InvalidUserId_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockTokenService.Setup(service => service.InvalidateTokenAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LogoutAsync_ExceptionThrownInTokenService_ThrowsException()
        {
            // Arrange
            var userId = _users[0].Id;
            _mockTokenService.Setup(service => service.InvalidateTokenAsync(userId)).Throws<Exception>();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.LogoutAsync(userId));
        }
    }
}