
using AutoMapper;

using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;

using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class AddressService : IAddressService
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
            var foundAddresses = await _addressRepo.GetAddressBookByUserIdAsync(userId);
            var foundAddressesReadDto = _mapper.Map<IEnumerable<AddressReadDto>>(foundAddresses);
            return foundAddressesReadDto;
        }

        public async Task<AddressReadDto> GetAddressByIdAsync(Guid addressId)
        {
            var foundAddress = await _addressRepo.GetAddressByIdAsync(addressId);
            var foundAddressReadDto = _mapper.Map<AddressReadDto>(foundAddress);
            return foundAddressReadDto;

        }
        public async Task<AddressReadDto> CreateAddressAsync(Guid UserId, AddressCreateDto addressCreateDto)
        {

            if (string.IsNullOrEmpty(addressCreateDto.Recipient)) throw AppException.InvalidInput("Recipient cannot be empty");
            if (string.IsNullOrEmpty(addressCreateDto.Phone)) throw AppException.InvalidInput("Phone cannot be empty");
            if (string.IsNullOrEmpty(addressCreateDto.Line1)) throw AppException.InvalidInput("Line1 cannot be empty");
            if (string.IsNullOrEmpty(addressCreateDto.PostalCode)) throw AppException.InvalidInput("PostalCode cannot be empty");
            if (string.IsNullOrEmpty(addressCreateDto.City)) throw AppException.InvalidInput("City cannot be empty");

            var newAddress = _mapper.Map<AddressCreateDto, Address>(addressCreateDto);
            newAddress.UserId = UserId;
            var createdAddress = await _addressRepo.CreateAddressAsync(newAddress);
            var createdAddressReadDto = _mapper.Map<AddressReadDto>(createdAddress);
            return createdAddressReadDto;

        }
        public async Task<AddressReadDto> UpdateAddressAsync(Guid addressId, AddressUpdateDto addressUpdateDto)
        {

            if (addressUpdateDto.Recipient == string.Empty) throw AppException.InvalidInput("Recipient cannot be empty");
            if (addressUpdateDto.Phone == string.Empty) throw AppException.InvalidInput("Phone cannot be empty");
            if (addressUpdateDto.Line1 == string.Empty) throw AppException.InvalidInput("Line1 cannot be empty");
            if (addressUpdateDto.PostalCode == string.Empty) throw AppException.InvalidInput("PostalCode cannot be empty");
            if (addressUpdateDto.City == string.Empty) throw AppException.InvalidInput("City cannot be empty");

            var foundAddress = await _addressRepo.GetAddressByIdAsync(addressId);
            foundAddress.Recipient = addressUpdateDto.Recipient ?? foundAddress.Recipient;
            foundAddress.Phone = addressUpdateDto.Phone ?? foundAddress.Phone;
            foundAddress.Line1 = addressUpdateDto.Line1 ?? foundAddress.Line1;
            foundAddress.Line2 = addressUpdateDto.Line2 ?? foundAddress.Line2;
            foundAddress.PostalCode = addressUpdateDto.PostalCode ?? foundAddress.PostalCode;
            foundAddress.City = addressUpdateDto.City ?? foundAddress.City;

            foundAddress.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            var updatedAddress = await _addressRepo.UpdateAddressByIdAsync(foundAddress);
            var UpdateAddressReadDto = _mapper.Map<AddressReadDto>(updatedAddress);

            return UpdateAddressReadDto;
        }
        public async Task<bool> DeleteAddressByIdAsync(Guid addressId)
        {
            if(addressId ==Guid.Empty) throw AppException.InvalidInput("Address id cannot be null");
            
            // if not ffound, repo will throw exception
           return await _addressRepo.DeleteAddressByIdAsync(addressId);
        }
    }
}