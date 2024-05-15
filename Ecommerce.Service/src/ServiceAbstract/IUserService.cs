using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync(UserQueryOptions userQueryOptions);
        Task<UserReadDto> GetUserByIdAsync(Guid userId);
        Task<UserReadDto> GetUserByEmailAsync(string email);
        Task<UserReadDto> CreateUserAsync(UserCreateDto userCreateDto); // this method will create User with address if there is
        Task<UserReadDto> UpdateUserByIdAsync(Guid userId, UserUpdateDto userUpdateDto); // this method doesn't include address, only user table
        Task<bool> DeleteUserByIdAsync(Guid userId); // this method will also delete address book
        Task<AddressReadDto> AddAddressByUserIdAsync(Guid userId, AddressCreateDto addressCreateDto); // this method only used for adding a new address record
        Task<AddressReadDto> UpdateAddressByIdAsync(Guid addressId, AddressUpdateDto addressUpdateDto); // this method only user for updating a single address record
        Task<bool> DeleteAddressByIdAsync (Guid addressId);
    }
}