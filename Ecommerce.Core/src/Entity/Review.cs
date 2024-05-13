using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Review : BaseEntity
    {
        [Column(TypeName = "integer")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [Column(TypeName = "text")]
        public string? Content { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; } // Foreign key navigate to user
        public User User { get; set; } = null!;

        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; } // Foreign key navigate to product
        public Product Product { get; set; } = null!;
    }
}
