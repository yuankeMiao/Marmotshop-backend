using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Review : BaseEntity
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public float Rating { get; set; }
        
        [Column(TypeName = "text")]
        public string? Content { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; } // Foreign key navigate to user
        public required User User { get; set; } // might remove this 

        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; } // Foreign key navigate to product
        public required Product Product { get; set; } // might remove this 
    }
}
