
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{

    public class UserReadDto : BaseEntity
    {
        public required string UserFirstname { get; set; }
        public required string UserLastname { get; set; }
        public required string UserEmail { get; set; }
        public string? UserAvatar { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class UserCreateDto
    {
        public required string UserFirstname { get; set; }
        public required string UserLastname { get; set; }
         public required string UserEmail { get; set; }
        public required string UserPassword { get; set; }
        public string? UserAvatar { get; set; }
        public UserRole UserRole { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
    }

    public class UserUpdateDto
    {
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public string? UserAvatar { get; set; }
        public UserRole? UserRole { get; set; }
    }
}