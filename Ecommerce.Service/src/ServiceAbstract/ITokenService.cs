using Ecommerce.Core.src.Entity;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface ITokenService
    {
        TokenResponseDto GetToken(User foundUser);
        TokenResponseDto RefreshToken(string token, User foundUser);
        Task<bool> InvalidateTokenAsync(Guid userId);
        bool IsTokenValid(RefreshToken token);
    }
}
