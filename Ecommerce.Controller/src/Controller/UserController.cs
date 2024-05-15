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
        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync([FromQuery] UserQueryOptions userQueryOptions)
        {
            return await _userService.GetAllUsersAsync(userQueryOptions);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId}")] // endpoint: /users/:user_id
        public async Task<UserReadDto> GetUserByIdAsync([FromRoute] Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [AllowAnonymous]
        [HttpPost()] // endpoint: /users
                     // public async Task<UserReadDto> CreateUserAsync([FromBody] UserCreateDto userCreateDto)
        public async Task<UserReadDto> CreateUserAsync([FromBody] UserCreateDto userCreateDto)
        {
            return await _userService.CreateUserAsync(userCreateDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}")] // endpoint: /users/:user_id
        public async Task<UserReadDto> UpdateUserByIdAsync([FromRoute] Guid userId, [FromBody] UserUpdateDto userUpdateDto)
        {
            return await _userService.UpdateUserByIdAsync(userId, userUpdateDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")] // endpoint: /users/:user_id
        public async Task<bool> DeleteUserByIdAsync([FromRoute] Guid userId)
        {
            return await _userService.DeleteUserByIdAsync(userId);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<UserReadDto> GetUserProfileAsync()
        {
            var claims = HttpContext.User;
            var userId = Guid.Parse(claims.FindFirst(ClaimTypes.NameIdentifier).Value);

            return await _userService.GetUserByIdAsync(userId);
        }

        [Authorize] // will implement authorization later
        [HttpPut("profile")]
        public async Task<UserReadDto> UpdateUserProfileAsync(UserUpdateDto userUpdateDto)
        {
            throw new NotImplementedException();
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
        [HttpPut("profile/addresses/{addressId}")]
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
    }
}
