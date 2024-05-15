
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressReadDto>> GetAddressBookByUserIdAsync(Guid userId);
        Task<AddressReadDto> CreateAddressAsync(Guid UserId, AddressCreateDto addressCreateDto);
        Task<AddressReadDto> UpdateAddressAsync(Guid addressId, AddressUpdateDto addressUpdateDto);
        Task<bool> DeleteAddressByIdAsync(Guid addressId);
    }
}