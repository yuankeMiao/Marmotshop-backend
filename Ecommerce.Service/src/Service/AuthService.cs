using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepo _userRepo;
        private ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        public AuthService(IUserRepo userRepo, ITokenService tokenService, IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }


        public async Task<TokenResponseDto> LoginAsync(UserCredential userCredential)
        {
            try
            {
                var foundUser = await _userRepo.GetUserByEmailAsync(userCredential.Email) ?? throw AppException.NotFound("Email is not registered");

                var isMatch = _passwordService.VerifyPassword(userCredential.Password, foundUser.Password, foundUser.Salt);
                if (isMatch)
                {
                    return _tokenService.GetToken(foundUser);
                }
                else
                {
                    throw AppException.InvalidLoginCredentials("Incorrect password");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, Guid userId)
        {
            try
            {
                var foundUser = await _userRepo.GetUserByIdAsync(userId) ?? throw AppException.NotFound("User not found");
                return _tokenService.RefreshToken(refreshToken, foundUser);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            try
            {
                return await _tokenService.InvalidateTokenAsync(userId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}