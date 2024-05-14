using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Entity
{
    public class User : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(20, MinimumLength = 2)]
        public required string Firstname { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(20, MinimumLength = 2)]
        public required string Lastname { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public required string Password { get; set; }
        [Required]
        [Column(TypeName = "bytea")]
        public required byte[] Salt { get; set; }

        [Column(TypeName = "varchar")]
        public string? Avatar { get; set; }
        public UserRole UserRole { get; set; }
        public HashSet<Address> Addresses { get; set; } = [];
    }
}