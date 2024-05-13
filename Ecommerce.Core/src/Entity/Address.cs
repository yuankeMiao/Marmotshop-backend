

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Address : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar")]
        public required string Line1 { get; set; }

        [Column(TypeName = "varchar")]
        public string? Line2 { get; set; }

        public int PostalCode { get; set; }

        [Column(TypeName = "varchar")]
        public required string City { get; set; }

        [ForeignKey("UserId")]
        public required string UserId { get; set; }
        public User User{ get; set; } = null!;
    }
}