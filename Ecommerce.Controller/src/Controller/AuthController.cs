using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;


        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> LoginAsync([FromBody] UserCredential userCredential)
        {
            var tokenResponse = await _authService.LoginAsync(userCredential);
            return Ok(tokenResponse);

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> RegisterAsync([FromBody] UserCreateDto userCreateDto)
        {

            userCreateDto.Role = UserRole.Customer; // this endpoint can only add customer users
            var user = await _userService.CreateUserAsync(userCreateDto);
            return StatusCode(201, user);
        }


        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var userId = GetUserIdClaim();
            var tokenResponse = await _authService.RefreshTokenAsync(requestDto.RefreshToken, userId);
            return Ok(tokenResponse);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = GetUserIdClaim();

            // Call the logout method
            var isLoggedOut = await _authService.LogoutAsync(userId);
            if (isLoggedOut)
            {
                return Ok("Logged out successfully");
            }
            else
            {
                return BadRequest("Failed to logout");
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