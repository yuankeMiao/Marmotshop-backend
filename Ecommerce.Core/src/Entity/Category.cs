using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Category : BaseEntity
    {

        [Required]
        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public required string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar")]
        public required string Image { get; set; }
    }
}