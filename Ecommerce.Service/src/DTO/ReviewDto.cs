using System.ComponentModel.DataAnnotations;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ReviewReadDto : BaseEntity
    {
        public int Rating { get; set; }
        public string? Content { get; set; }
        public required UserReadDto User { get; set; }
        public required string ProductId { get; set; }
         public required string ProductTitle { get; set; }
    }
    public class ReviewCreateDto
    {
        public int Rating { get; set; }
        public string? Content { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
    public class ReviewUpdateDto
    {
        public int? Rating { get; set; }
        public string? Content { get; set; }
    }
}