
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{

    public class UserReadDto : BaseEntity
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public string? Avatar { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserCreateDto
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
         public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Avatar { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
    }

    public class UserUpdateDto
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
    }
}