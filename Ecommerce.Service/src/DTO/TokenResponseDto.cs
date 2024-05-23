
namespace Ecommerce.Service.src.DTO
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }

    public class RefreshToken
    {
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public required string RefreshToken { get; set; }
    }

}