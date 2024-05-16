using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private IAuthorizationService _authorizationService;

        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet] // endpoint: /users
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsersAsync([FromQuery] UserQueryOptions userQueryOptions)
        {
            var users = await _userService.GetAllUsersAsync(userQueryOptions);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId}")] // endpoint: /users/:user_id
        public async Task<ActionResult<UserReadDto>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost()] // endpoint: /users
        public async Task<ActionResult<UserReadDto>> CreateUserAsync([FromBody] UserCreateDto userCreateDto)
        {
            var user = await _userService.CreateUserAsync(userCreateDto);
            return Created($"http://localhost:5227/api/v1/users/{user.Id}", user);
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
        public async Task<ActionResult<UserReadDto>> UpdateUserProfileAsync(UserUpdateDto userUpdateDto)
        {
            var userId = GetUserIdClaim();
            var user = await _userService.UpdateUserByIdAsync(userId, userUpdateDto);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("profile/addresses")]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> GetAddressBookByUserIdAsync()
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpPost("profile/addresses")]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> AddAddressAsync([FromBody] AddressCreateDto addressCreateDto)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpPatch("profile/addresses/{addressId}")]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> UpdateAddressByIdAsync([FromRoute] Guid addressId, [FromBody] AddressUpdateDto addressUpdateDto)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpDelete("profile/addresses/{addressId}")]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> DeleteAddressByIdAsync([FromRoute] Guid addressId, [FromBody] AddressUpdateDto addressUpdateDto)
        {
            throw new NotImplementedException();
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
