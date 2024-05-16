

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Address : BaseEntity
    {
        [Required]
        public required string Recipient { get; set; }

        [Required]
        [Phone]
        public required string Phone { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public required string Line1 { get; set; }

        [Column(TypeName = "varchar")]
        public string? Line2 { get; set; }

        [Required]
        public required string PostalCode { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public required string City { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public required Guid UserId { get; set; }
        public User User { get; set; } = null!; // reference
    }
}