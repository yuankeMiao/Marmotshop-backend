using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IAuthService
    {
        // should return two strings - accessToken and refreshToken
        Task<TokenResponseDto> LoginAsync(UserCredential userCredential);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, Guid userId);
        Task<bool> LogoutAsync(Guid userId);
    }
}