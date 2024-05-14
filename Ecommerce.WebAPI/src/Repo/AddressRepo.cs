
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class AddressRepo : IAddressRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Address> _addresses;
        public AddressRepo(AppDbContext context)
        {
            _context = context;
            _addresses = _context.Addresses;
        }

        public async Task<IEnumerable<Address>> GetAddressBookByUserIdAsync(Guid userId)
        {
            var foundAddresses = await _addresses.Where(a => a.UserId == userId).ToListAsync();
            return foundAddresses;
        }
        public async Task<Address> GetAddressByIdAsync(Guid AddressId)
        {
            var foundAddress = await _addresses.FindAsync(AddressId) ?? throw AppException.NotFound("Address not found");
            return foundAddress;
        }
        public async Task<Address> CreateAddressAsync(Address newAddress)
        {
            await _addresses.AddAsync(newAddress);
            await _context.SaveChangesAsync();
            return newAddress;
        }
        public async Task<Address> UpdateAddressByIdAsync(Address updatedAddress)
        {
            _addresses.Update(updatedAddress);
            await _context.SaveChangesAsync();
            return updatedAddress;
        }
        public async Task<bool> DeleteAddressByIdAsync(Guid AddressId)
        {
            var foundAddress = await _addresses.FindAsync(AddressId) ?? throw AppException.NotFound("Address not found");
            _addresses.Remove(foundAddress);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}