
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IAddressRepo
    {
        Task<IEnumerable<Address>> GetAddressBookByUserIdAsync(Guid userId);
        Task<Address> GetAddressByIdAsync(Guid AddressId);
        Task<Address> CreateAddressAsync(Address newAddress);
        Task<Address> UpdateAddressByIdAsync(Address updatedAddress);
        Task<bool> DeleteAddressByIdAsync(Guid AddressId);
    }
}