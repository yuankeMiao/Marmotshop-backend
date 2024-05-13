

using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Address: BaseEntity
    {
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public int PostalCode { get; set; }
        public required string City { get; set; }
        [ForeignKey("UserId")]
        public required string UserId { get; set;}
    }
}