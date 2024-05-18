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
    public class AddressServiceTests
    {
        private readonly Mock<IAddressRepo> _mockAddressRepo;
        private readonly IMapper _mapper;
        private readonly AddressService _addressService;
        private readonly List<Address> _addresses;
        private readonly List<User> _users;

        public AddressServiceTests()
        {
            _mockAddressRepo = new Mock<IAddressRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });

            _mapper = config.CreateMapper();
            _addressService = new AddressService(_mockAddressRepo.Object, _mapper);
            _users = SeedingData.GetUsers();
            _addresses = SeedingData.GetAddresses(_users);
        }

        [Fact]
        public async Task GetAddressBookByUserIdAsync_ReturnsAddressReadDtos()
        {
            // Arrange
            var userId = _users[0].Id;
            var addressBook = _addresses.Where(a => a.UserId == userId).ToList();

            _mockAddressRepo.Setup(repo => repo.GetAddressBookByUserIdAsync(userId)).ReturnsAsync(addressBook);
            var expectedDtos = _mapper.Map<List<AddressReadDto>>(addressBook);

            // Act
            var result = await _addressService.GetAddressBookByUserIdAsync(userId);
            var resultList = result.ToList();

            // Assert
            Assert.Equal(expectedDtos.Count, resultList.Count);
            for (int i = 0; i < expectedDtos.Count; i++)
            {
                AssertAddressReadDtoEqual(expectedDtos[i], resultList[i]);
            }
        }

        [Fact]
        public async Task GetAddressByIdAsync_ValidId_ReturnsAddressReadDto()
        {
            // Arrange
            var addressId = _addresses[0].Id;
            var expectedDto = _mapper.Map<AddressReadDto>(_addresses[0]);

            _mockAddressRepo.Setup(repo => repo.GetAddressByIdAsync(addressId)).ReturnsAsync(_addresses[0]);

            // Act
            var result = await _addressService.GetAddressByIdAsync(addressId);

            // Assert
            AssertAddressReadDtoEqual(expectedDto, result);
        }

        [Fact]
        public async Task GetAddressByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var addressId = Guid.NewGuid();

            _mockAddressRepo.Setup(repo => repo.GetAddressByIdAsync(addressId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _addressService.GetAddressByIdAsync(addressId));
        }

        [Fact]
        public async Task CreateAddressAsync_ValidData_ReturnsAddressReadDto()
        {
            // Arrange
            var userId = _users[0].Id;
            var addressCreateDto = new AddressCreateDto
            {
                Recipient = "John Doe",
                Phone = "1234567890",
                Line1 = "123 Main St",
                Line2 = "Apt 4",
                PostalCode = "12345",
                City = "Anytown"
            };
            var newAddress = _mapper.Map<Address>(addressCreateDto);
            newAddress.UserId = userId;
            var createdAddress = new Address
            {
                Id = Guid.NewGuid(),
                Recipient = newAddress.Recipient,
                Phone = newAddress.Phone,
                Line1 = newAddress.Line1,
                Line2 = newAddress.Line2,
                PostalCode = newAddress.PostalCode,
                City = newAddress.City,
                UserId = userId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var expectedDto = _mapper.Map<AddressReadDto>(createdAddress);

            _mockAddressRepo.Setup(repo => repo.CreateAddressAsync(It.IsAny<Address>())).ReturnsAsync(createdAddress);

            // Act
            var result = await _addressService.CreateAddressAsync(userId, addressCreateDto);

            // Assert
            AssertAddressReadDtoEqual(expectedDto, result);
        }

        [Theory]
        [InlineData("", "1234567890", "123 Main St", "Apt 4", "12345", "Anytown")]
        [InlineData("John Doe", "", "123 Main St", "Apt 4", "12345", "Anytown")]
        [InlineData("John Doe", "1234567890", "", "Apt 4", "12345", "Anytown")]
        public async Task CreateAddressAsync_InvalidData_ThrowsException(string recipient, string phone, string line1, string line2, string postalCode, string city)
        {
            // Arrange
            var userId = _users[0].Id;
            var invalidAddressCreateDto = new AddressCreateDto
            {
                Recipient = recipient,
                Phone = phone,
                Line1 = line1,
                Line2 = line2,
                PostalCode = postalCode,
                City = city
            };

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _addressService.CreateAddressAsync(userId, invalidAddressCreateDto));
        }

        [Fact]
        public async Task UpdateAddressAsync_ValidData_ReturnsUpdatedAddressReadDto()
        {
            // Arrange
            var addressId = _addresses[0].Id;
            var addressUpdateDto = new AddressUpdateDto { Recipient = "Jane Doe", Phone = "0987654321", Line1 = "456 Elm St" };
            var updatedAddress = _addresses[0];
            updatedAddress.Recipient = addressUpdateDto.Recipient;
            updatedAddress.Phone = addressUpdateDto.Phone;
            updatedAddress.Line1 = addressUpdateDto.Line1;
            updatedAddress.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            var expectedDto = _mapper.Map<AddressReadDto>(updatedAddress);

            _mockAddressRepo.Setup(repo => repo.GetAddressByIdAsync(addressId)).ReturnsAsync(_addresses[0]);
            _mockAddressRepo.Setup(repo => repo.UpdateAddressByIdAsync(It.IsAny<Address>())).ReturnsAsync(updatedAddress);

            // Act
            var result = await _addressService.UpdateAddressAsync(addressId, addressUpdateDto);

            // Assert
            AssertAddressReadDtoEqual(expectedDto, result);
        }

        [Theory]
        [InlineData("", "0987654321", "456 Elm St", "Apt 5", "54321", "Othertown")]
        [InlineData("Jane Doe", "", "456 Elm St", "Apt 5", "54321", "Othertown")]
        [InlineData("Jane Doe", "0987654321", "", "Apt 5", "54321", "Othertown")]
        public async Task UpdateAddressAsync_InvalidData_ThrowsException(string recipient, string phone, string line1, string line2, string postalCode, string city)
        {
            // Arrange
            var addressId = _addresses[0].Id;
            var invalidAddressUpdateDto = new AddressUpdateDto
            {
                Recipient = recipient,
                Phone = phone,
                Line1 = line1,
                Line2 = line2,
                PostalCode = postalCode,
                City = city
            };

            _mockAddressRepo.Setup(repo => repo.GetAddressByIdAsync(addressId)).ReturnsAsync(_addresses[0]);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await _addressService.UpdateAddressAsync(addressId, invalidAddressUpdateDto));
        }

        [Fact]
        public async Task DeleteAddressByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var addressId = _addresses[0].Id;

            _mockAddressRepo.Setup(repo => repo.DeleteAddressByIdAsync(addressId)).ReturnsAsync(true);

            // Act
            var result = await _addressService.DeleteAddressByIdAsync(addressId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAddressByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var addressId = Guid.NewGuid();

            _mockAddressRepo.Setup(repo => repo.DeleteAddressByIdAsync(addressId)).ThrowsAsync(AppException.NotFound());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _addressService.DeleteAddressByIdAsync(addressId));
        }

        private void AssertAddressReadDtoEqual(AddressReadDto expected, AddressReadDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Recipient, actual.Recipient);
            Assert.Equal(expected.Phone, actual.Phone);
            Assert.Equal(expected.Line1, actual.Line1);
            Assert.Equal(expected.Line2, actual.Line2);
            Assert.Equal(expected.PostalCode, actual.PostalCode);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.CreatedDate, actual.CreatedDate);
            // Adjust the UpdatedDate comparison to handle the scenario where one is null
            if (expected.UpdatedDate == null)
            {
                Assert.Null(actual.UpdatedDate);
            }
            else
            {
                Assert.Equal(expected.UpdatedDate, actual.UpdatedDate);
            }
        }
    }
}
