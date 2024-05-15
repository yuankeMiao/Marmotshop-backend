using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync(UserQueryOptions userQueryOptions);
        Task<UserReadDto> GetUserByIdAsync(Guid userId);
        Task<UserReadDto> GetUserByEmailAsync(string email);
        Task<UserReadDto> CreateUserAsync(UserCreateDto userCreateDto); // this method will not create address
        Task<UserReadDto> UpdateUserByIdAsync(Guid userId, UserUpdateDto userUpdateDto); // this method doesn't include address, only user table
        Task<bool> DeleteUserByIdAsync(Guid userId); // this method will also delete address book
    }
}