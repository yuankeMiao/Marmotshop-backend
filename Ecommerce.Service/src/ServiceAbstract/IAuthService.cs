using Ecommerce.Core.src.Common;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IAuthService
    {
        // should return two strings - accessToken and refreshToken
        Task<string> LoginAsync(UserCredential userCredential);
        Task<string> LogoutAsync();
    }
}