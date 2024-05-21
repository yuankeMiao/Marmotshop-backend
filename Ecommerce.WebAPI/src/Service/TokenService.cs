
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.Core.src.Entity;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.Extensions.Caching.Memory;
using Ecommerce.Service.src.DTO;
using System.Security.Cryptography;

namespace Ecommerce.WebAPI.src.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public TokenService(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public TokenResponseDto GetToken(User foundUser)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, foundUser.Email),
                new(ClaimTypes.NameIdentifier, foundUser.Id.ToString()),
                new(ClaimTypes.Role, foundUser.Role.ToString()),
            };

            // secret key
            var jwtKey = _configuration["Secrets:JwtKey"] ?? throw new ArgumentNullException("JwtKey is not found in appsettings.json");
            var securityKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature);

            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = securityKey,
                Issuer = _configuration["Secrets:Issuer"],
            };
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            var refreshToken = GenerateRefreshToken();
            _cache.Set($"RefreshToken_{foundUser.Id}", refreshToken);

            return new TokenResponseDto
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshToken.Token
            };
        }

        private static RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }

        public TokenResponseDto RefreshToken(string refreshToken, User foundUser)
        {
            var refreshTokenKey = $"RefreshToken_{foundUser.Id}";
            var storedRefreshToken = _cache.Get<RefreshToken>(refreshTokenKey);

            if (storedRefreshToken == null || storedRefreshToken.Token != refreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            if (storedRefreshToken.Revoked != null)
            {
                throw new SecurityTokenException("Refresh token has been revoked");
            }

            if (storedRefreshToken.Expires < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Refresh token has expired");
            }

            var newAccessToken = GetToken(foundUser).AccessToken;
            var newRefreshToken = GenerateRefreshToken();

            _cache.Set(refreshTokenKey, newRefreshToken);

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task<bool> InvalidateTokenAsync(Guid userId)
        {
            var refreshTokenKey = $"RefreshToken_{userId}";
            var storedToken = _cache.Get<RefreshToken>(refreshTokenKey);
            if (storedToken != null)
            {
                storedToken.Revoked = DateTime.UtcNow;
                _cache.Set(refreshTokenKey, storedToken);
                await Task.CompletedTask;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTokenValid(RefreshToken token)
        {
            if (token.Revoked != null)
            {
                return false; // Token is invalid because it has been revoked
            }

            if (token.Expires < DateTime.UtcNow)
            {
                return false; // Token is invalid because it has expired
            }

            return true; // Token is valid
        }

    }
}

