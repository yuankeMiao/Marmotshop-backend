
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Product : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar")]
        public required string Title { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public int DiscountPercentage { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public decimal? Rating { get; set; } // everytime a new review is added, it will trigger a function to calculate and change this number

        public int Stock { get; set; } = 0;

        [Column(TypeName = "varchar")]
        public string? Brand { get; set; }

        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; } //  foreign key navigate to category
        public Category Category { get; set; } = null!; // for reference only
        public required string Thumbnail { get; set; }
        public IEnumerable<Image>? Images { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}