using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class ProductImage : BaseEntity
    {
        [Column(TypeName = "varchar")]
        public required string Url { get; set; }
        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; } // Foreign key navigate to product
        public Product Product { get; set; } = null!;
    }
}