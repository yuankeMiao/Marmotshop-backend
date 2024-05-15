
using AutoMapper;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class AddressService: IAddressService
    {
        private readonly IAddressRepo _addressRepo;
        private readonly IMapper _mapper;
        public AddressService(IAddressRepo addressRepo, IMapper mapper)
        {
            _addressRepo = addressRepo;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AddressReadDto>> GetAddressBookByUserIdAsync(Guid userId)
        {
            try
            {
                var foundAddresses = await _addressRepo.GetAddressBookByUserIdAsync(userId);
                var foundAddressesReadDto = foundAddresses.Select(a => _mapper.Map<Address, AddressReadDto>(a));
                return foundAddressesReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AddressReadDto> CreateAddressAsync(Guid UserId, AddressCreateDto addressCreateDto)
        {
            try
            {
                var newAddress = _mapper.Map<AddressCreateDto, Address>(addressCreateDto);
                newAddress.UserId = UserId;
                var createdAddress = await _addressRepo.CreateAddressAsync(newAddress);
                var createdAddressReadDto = _mapper.Map<Address, AddressReadDto>(createdAddress);
                return createdAddressReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AddressReadDto> UpdateAddressAsync(Guid addressId, AddressUpdateDto addressUpdateDto)
        {
            try
            {
                var foundAddress = await _addressRepo.GetAddressByIdAsync(addressId);
                foundAddress.Recipient = addressUpdateDto.Recipient ?? foundAddress.Recipient;
                foundAddress.Phone = addressUpdateDto.Phone ?? foundAddress.Phone;
                foundAddress.Line1 = addressUpdateDto.Line1 ?? foundAddress.Line1;
                foundAddress.Line2 = addressUpdateDto.Line2 ?? foundAddress.Line2;
                foundAddress.PostalCode = addressUpdateDto.PostalCode ?? foundAddress.PostalCode;
                foundAddress.City = addressUpdateDto.City ?? foundAddress.City;

                var updatedAddress = await _addressRepo.UpdateAddressByIdAsync(foundAddress);
                var UpdateAddressReadDto = _mapper.Map<Address, AddressReadDto>(updatedAddress);

                return UpdateAddressReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteAddressByIdAsync(Guid addressId)
        {
            try
            {
                // if not ffound, repo will throw exception
                var foundAddress = await _addressRepo.GetAddressByIdAsync(addressId);
                await _addressRepo.DeleteAddressByIdAsync(addressId);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}