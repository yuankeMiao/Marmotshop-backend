
using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private IAuthorizationService _authorizationService;

        public UserController(IUserService userService, IAddressService addressService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _addressService = addressService;
            _authorizationService = authorizationService;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet] // endpoint: /users
        public async Task<ActionResult<QueryResult<UserReadDto>>> GetAllUsersAsync([FromQuery] UserQueryOptions? options)
        {
            var result = await _userService.GetAllUsersAsync(options);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId}")] // endpoint: /users/:user_id
        public async Task<ActionResult<UserReadDto>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")] // this endpoint is not user register, it's only for admin to add user manually
        [HttpPost()]
        public async Task<ActionResult<UserReadDto>> CreateUserAsync([FromBody] UserCreateDto userCreateDto)
        {

            if (userCreateDto.Role == UserRole.Admin)
            {
                //only super admin can create admin user
                var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, null, "SuperAdmin");
                if (!authResult.Succeeded) return Forbid();
            }
            var user = await _userService.CreateUserAsync(userCreateDto);
            return StatusCode(201, user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{userId}")] // endpoint: /users/:user_id
        public async Task<ActionResult<UserReadDto>> UpdateUserByIdAsync([FromRoute] Guid userId, [FromBody] UserUpdateDto userUpdateDto)
        {
            var user = await _userService.UpdateUserByIdAsync(userId, userUpdateDto);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")] // endpoint: /users/:user_id
        public async Task<ActionResult<bool>> DeleteUserByIdAsync([FromRoute] Guid userId)
        {
            var deleted = await _userService.DeleteUserByIdAsync(userId);
            return Ok(deleted);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserReadDto>> GetUserProfileAsync()
        {
            var userId = GetUserIdClaim();

            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [Authorize] // will implement authorization later
        [HttpPatch("profile")]
        public async Task<ActionResult<UserReadDto>> UpdateUserProfileAsync([FromBody] UserUpdateDto userUpdateDto)
        {
            var userId = GetUserIdClaim();
            var user = await _userService.UpdateUserByIdAsync(userId, userUpdateDto);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("profile/addresses")]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> GetAddressBookByUserIdAsync()
        {
            var userId = GetUserIdClaim();
            var addressBook = await _addressService.GetAddressBookByUserIdAsync(userId);

            return Ok(addressBook);
        }

        [Authorize]
        [HttpGet("profile/addresses/{addressId}")]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> GetAddressByIdAsync([FromRoute] Guid addressId)
        {
            var userId = GetUserIdClaim();
            var address = await _addressService.GetAddressByIdAsync(addressId);

            if (userId != address.UserId) return Forbid();
            return Ok(address);
        }

        [Authorize]
        [HttpPost("profile/addresses")]
        public async Task<ActionResult<AddressReadDto>> AddAddressAsync([FromBody] AddressCreateDto addressCreateDto)
        {
            var userId = GetUserIdClaim();
            var address = await _addressService.CreateAddressAsync(userId, addressCreateDto);
            return Ok(address);
        }

        [Authorize]
        [HttpPatch("profile/addresses/{addressId}")]
        public async Task<ActionResult<AddressReadDto>> UpdateAddressByIdAsync([FromRoute] Guid addressId, [FromBody] AddressUpdateDto addressUpdateDto)
        {
            var foundAddress = await _addressService.GetAddressByIdAsync(addressId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundAddress, "AddressOwner");

            if (authResult.Succeeded)
            {
                var address = await _addressService.UpdateAddressAsync(addressId, addressUpdateDto);
                return Ok(address);
            }
            else
            {
                return Forbid();
            }

        }

        [Authorize]
        [HttpDelete("profile/addresses/{addressId}")]
        public async Task<ActionResult<bool>> DeleteAddressByIdAsync([FromRoute] Guid addressId)
        {
            var foundAddress = await _addressService.GetAddressByIdAsync(addressId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundAddress, "AddressOwner");

            if (authResult.Succeeded)
            {
                var deleted = await _addressService.DeleteAddressByIdAsync(addressId);
                return Ok(deleted);
            }
            else
            {
                return Forbid();
            }
        }

        private Guid GetUserIdClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found");
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID format");
            }
            return userId;
        }
    }
}
