using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> LoginAsync([FromBody] UserCredential userCredential)
        {
            var tokenResponse = await _authService.LoginAsync(userCredential);
            return Ok(tokenResponse);

        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var tokenResponse = await _authService.RefreshTokenAsync(requestDto.RefreshToken, requestDto.UserId);
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