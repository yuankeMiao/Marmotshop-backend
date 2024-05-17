using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Core.src.RepoAbstract;
using AutoMapper;
using Ecommerce.Service.src.Shared;

namespace Ecommerce.Service.src.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;


        public UserService(IMapper mapper, IUserRepo userRepo, IPasswordService passwordService)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _passwordService = passwordService;
        }
        public async Task<QueryResult<UserReadDto>> GetAllUsersAsync(UserQueryOptions? options)
        {
            try
            {
                var queryResult = await _userRepo.GetAllUsersAsync(options);
                var users = queryResult.Data;

                var userReadDtos = _mapper.Map<IEnumerable<UserReadDto>>(users);
                var totalCount = queryResult.TotalCount;

                return  new QueryResult<UserReadDto> { Data = userReadDtos, TotalCount = totalCount };
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
                AppException.InvalidInput("UserId is required");
            }
            try
            {
                var foundUser = await _userRepo.GetUserByIdAsync(userId);
                var userReadDto = _mapper.Map<User, UserReadDto>(foundUser);

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
                throw AppException.InvalidInput("Email is required");
            }
            try
            {
                var foundUser = await _userRepo.GetUserByEmailAsync(email);
                var userReadDto = _mapper.Map<User, UserReadDto>(foundUser);

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
                if (string.IsNullOrWhiteSpace(userCreateDto.Firstname)) throw AppException.InvalidInput("User name cannot be empty");
                if (userCreateDto.Firstname.Length > 20) throw AppException.InvalidInput("User name cannot be longer than 20 characters");
                if (string.IsNullOrWhiteSpace(userCreateDto.Lastname)) throw AppException.InvalidInput("User name cannot be empty");
                if (userCreateDto.Lastname.Length > 20) throw AppException.InvalidInput("User name cannot be longer than 20 characters");

                if (!ValidationHelper.IsEmailValid(userCreateDto.Email)) throw AppException.InvalidInput("Email is not valid");
                if (!ValidationHelper.IsValidPassword(userCreateDto.Password)) throw AppException.InvalidInput("Password shoulbe 6-20 charaters with at least one uppercase, one lowercase, and one number");
                if (userCreateDto.Avatar is not null && !ValidationHelper.IsImageUrlValid(userCreateDto.Avatar)) throw AppException.InvalidInput("Image must be a url");

                // Create a new User entity and populate its properties from the UserCreateDto

                var newUser = _mapper.Map<UserCreateDto, User>(userCreateDto);
                // Call the CreateUserAsync method of the repository to create the user

                // encrypt the password
                newUser.Password = _passwordService.HashPassword(newUser.Password, out byte[] salt);
                newUser.Salt = salt;

                var createdUser = await _userRepo.CreateUserAsync(newUser);
                var createdUserDto = _mapper.Map<User, UserReadDto>(createdUser);
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
                if (userUpdateDto.Firstname is not null)
                {
                    if (!ValidationHelper.IsValidName(userUpdateDto.Firstname)) throw AppException.InvalidInput("Firstname should be 2 - 20 characters without numbers");
                    foundUser.Firstname = userUpdateDto.Firstname;
                }

                if (userUpdateDto.Lastname is not null)
                {
                    if (!ValidationHelper.IsValidName(userUpdateDto.Lastname)) throw AppException.InvalidInput("Lastname should be 2 - 20 characters without numbers");
                    foundUser.Lastname = userUpdateDto.Lastname;
                }

                if (userUpdateDto.Email is not null)
                {
                    if (!ValidationHelper.IsEmailValid(userUpdateDto.Email)) throw AppException.InvalidInput("Email is not valid");
                    foundUser.Email = userUpdateDto.Email;
                }

                if (userUpdateDto.Password is not null)
                {
                    if (!ValidationHelper.IsValidPassword(userUpdateDto.Password))
                    {
                        throw AppException.InvalidInput("Password shoulbe 6-20 charaters with at least one uppercase, one lowercase, and one number");
                    }

                    foundUser.Password = _passwordService.HashPassword(userUpdateDto.Password, out byte[] salt);
                    foundUser.Salt = salt;
                }

                if (userUpdateDto.Avatar is not null)
                {
                    if (!ValidationHelper.IsImageUrlValid(userUpdateDto.Avatar)) throw AppException.InvalidInput("Image must be a url");
                    foundUser.Avatar = userUpdateDto.Avatar;
                }

                foundUser.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

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
    }
}