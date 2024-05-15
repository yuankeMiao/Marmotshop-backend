using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Core.src.RepoAbstract;
using AutoMapper;
using System.Text.RegularExpressions;

namespace Ecommerce.Service.src.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IAddressRepo _addressRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;


        public UserService(IMapper mapper, IUserRepo userRepo, IAddressRepo addressRepo, IPasswordService passwordService)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _addressRepo = addressRepo;
            _passwordService = passwordService;
        }
        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync(UserQueryOptions userQueryOptions)
        {
            try
            {
                var users = await _userRepo.GetAllUsersAsync(userQueryOptions);
                var UserReadDtos = users.Select(u => _mapper.Map<User, UserReadDto>(u));

                foreach (var userReadDto in UserReadDtos)
                {
                    var addressBook = await _addressRepo.GetAddressBookByUserIdAsync(userReadDto.Id);
                    userReadDto.Addresses = _mapper.Map<IEnumerable<Address>, HashSet<AddressReadDto>>(addressBook);
                }

                return UserReadDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserReadDto> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                AppException.BadRequest("UserId is required");
            }
            try
            {
                var foundUser = await _userRepo.GetUserByIdAsync(userId);
                var userReadDto = _mapper.Map<User, UserReadDto>(foundUser);

                var addressBook = await _addressRepo.GetAddressBookByUserIdAsync(userReadDto.Id);
                userReadDto.Addresses = _mapper.Map<IEnumerable<Address>, HashSet<AddressReadDto>>(addressBook);

                return userReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserReadDto> GetUserByEmailAsync(string email)
        {
            if (email == string.Empty)
            {
                throw AppException.BadRequest("Email is required");
            }
            try
            {
                var foundUser = await _userRepo.GetUserByEmailAsync(email);
                var userReadDto = _mapper.Map<User, UserReadDto>(foundUser);

                var addressBook = await _addressRepo.GetAddressBookByUserIdAsync(userReadDto.Id);
                userReadDto.Addresses = _mapper.Map<IEnumerable<Address>, HashSet<AddressReadDto>>(addressBook);

                return userReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserReadDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            try
            {
                // validation
                if (string.IsNullOrEmpty(userCreateDto.Firstname)) throw AppException.InvalidInputException("User name cannot be empty");
                if (userCreateDto.Firstname.Length > 20) throw AppException.InvalidInputException("User name cannot be longer than 20 characters");

                if (string.IsNullOrEmpty(userCreateDto.Lastname)) throw AppException.InvalidInputException("User name cannot be empty");
                if (userCreateDto.Lastname.Length > 20) throw AppException.InvalidInputException("User name cannot be longer than 20 characters");

                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                // Create Regex object
                Regex emailRegex = new(emailPattern);
                if (!emailRegex.IsMatch(userCreateDto.Email)) throw AppException.InvalidInputException("Email is not valid");

                string imagePatten = @"^.*\.(jpg|jpeg|png|gif|bmp)$";
                Regex imageRegex = new(imagePatten);
                if (userCreateDto.Avatar is not null && !imageRegex.IsMatch(userCreateDto.Avatar)) throw AppException.InvalidInputException("Avatar can only be jpg|jpeg|png|gif|bmp");

                // Create a new User entity and populate its properties from the UserCreateDto

                var newUser = _mapper.Map<UserCreateDto, User>(userCreateDto);
                // Call the CreateUserAsync method of the repository to create the user

                // encrypt the password
                newUser.Password = _passwordService.HashPassword(newUser.Password, out byte[] salt);
                newUser.Salt = salt;

                if (userCreateDto.Addresses is not null)
                {
                    var newAddressBook = new HashSet<Address>();
                    foreach (var addressCreateDto in userCreateDto.Addresses)
                    {
                        var newAddress = _mapper.Map<AddressCreateDto, Address>(addressCreateDto);
                        var createdAddress = await _addressRepo.CreateAddressAsync(newAddress);
                        newAddressBook.Add(createdAddress);
                    }
                    newUser.Addresses = newAddressBook;
                }

                var createdUser = await _userRepo.CreateUserAsync(newUser);
                var createdUserDto = _mapper.Map<User, UserReadDto>(createdUser);
                createdUserDto.Addresses = _mapper.Map<HashSet<Address>, HashSet<AddressReadDto>>(newUser.Addresses);

                return createdUserDto;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<UserReadDto> UpdateUserByIdAsync(Guid userId, UserUpdateDto userUpdateDto)
        {
            try
            {
                var foundUser = await _userRepo.GetUserByIdAsync(userId);
                // validation
                if (userUpdateDto.Firstname is not null && string.IsNullOrEmpty(userUpdateDto.Firstname))
                {
                    throw AppException.InvalidInputException("Firstname cannot be empty");
                }

                if (userUpdateDto.Firstname is not null && userUpdateDto.Firstname.Length > 20)
                {
                    throw AppException.InvalidInputException("Firstname cannot be longer than 20 characters");
                }

                if (userUpdateDto.Lastname is not null && string.IsNullOrEmpty(userUpdateDto.Lastname))
                {
                    throw AppException.InvalidInputException("Lastname cannot be empty");
                }

                if (userUpdateDto.Lastname is not null && userUpdateDto.Lastname.Length > 20)
                {
                    throw AppException.InvalidInputException("Lastname cannot be longer than 20 characters");
                }


                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                // Create Regex object
                Regex emailRegex = new(emailPattern);
                if (userUpdateDto.Email is not null && !emailRegex.IsMatch(userUpdateDto.Email)) throw AppException.InvalidInputException("Email is not valid");

                string imagePatten = @"^.*\.(jpg|jpeg|png|gif|bmp)$";
                Regex imageRegex = new(imagePatten);
                if (userUpdateDto.Avatar is not null && !imageRegex.IsMatch(userUpdateDto.Avatar)) throw AppException.InvalidInputException("Avatar can only be jpg|jpeg|png|gif|bmp");

                foundUser.Firstname = userUpdateDto.Firstname ?? foundUser.Firstname;
                foundUser.Lastname = userUpdateDto.Lastname ?? foundUser.Lastname;
                foundUser.Email = userUpdateDto.Email ?? foundUser.Email;
                foundUser.Password = userUpdateDto.Password ?? foundUser.Password;
                foundUser.Avatar = userUpdateDto.Avatar ?? foundUser.Avatar;
                foundUser.Role = userUpdateDto.Role ?? foundUser.Role;

                // Update the user entity with the new values
                var updateUser = await _userRepo.UpdateUserByIdAsync(foundUser);

                // Map the updated user entity back to a UserReadDto
                var updatedUserDto = _mapper.Map<User, UserReadDto>(updateUser);

                return updatedUserDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteUserByIdAsync(Guid userId)
        {
            try
            {
                // Delete the user entity from the repository
                await _userRepo.DeleteUserByIdAsync(userId);
                // Return true to indicate successful deletion
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AddressReadDto> AddAddressByUserIdAsync(Guid userId, AddressCreateDto addressCreateDto)
        {
            try
            {
                // find the user
                var foundUser = await _userRepo.GetUserByIdAsync(userId);

                // add the address to db
                var newAddress = _mapper.Map<AddressCreateDto, Address>(addressCreateDto);
                newAddress.UserId = userId;
                var createdAddress = await _addressRepo.CreateAddressAsync(newAddress);

                // return the new address
                var createdAddressReadDto = _mapper.Map<Address, AddressReadDto>(createdAddress);
                return createdAddressReadDto;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<AddressReadDto> UpdateAddressByIdAsync(Guid addressId, AddressUpdateDto addressUpdateDto)
        {
            try
            {
                // find the address
                var foundAddress = await _addressRepo.GetAddressByIdAsync(addressId);

                foundAddress.Recipient = addressUpdateDto.Recipient ?? foundAddress.Recipient;
                foundAddress.Phone = addressUpdateDto.Phone ?? foundAddress.Phone;
                foundAddress.Line1 = addressUpdateDto.Line1 ?? foundAddress.Line1;
                foundAddress.Line2 = addressUpdateDto.Line2 ?? foundAddress.Line2;
                foundAddress.PostalCode = addressUpdateDto.PostalCode ?? foundAddress.PostalCode;
                foundAddress.City = addressUpdateDto.City ?? foundAddress.City;

                var updatedAddress = await _addressRepo.UpdateAddressByIdAsync(foundAddress);
                var updatedAddressReadDto = _mapper.Map<Address, AddressReadDto>(updatedAddress);
                return updatedAddressReadDto;
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
                // Delete the user entity from the repository
                await _addressRepo.DeleteAddressByIdAsync(addressId);
                // Return true to indicate successful deletion
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}