using System.ComponentModel.DataAnnotations;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ReviewReadDto : BaseEntity
    {
        public int ReviewRating { get; set; }
        public string? ReviewContent { get; set; }
        public required UserReadDto User { get; set; }
        public required ProductReviewReadDto Product { get; set; }
    }
    public class ReviewCreateDto
    {
        public int ReviewRating { get; set; }
        public string? ReviewContent { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
    public class ReviewUpdateDto
    {
        public int? ReviewRating { get; set; }
        public string? ReviewContent { get; set; }
    }
}