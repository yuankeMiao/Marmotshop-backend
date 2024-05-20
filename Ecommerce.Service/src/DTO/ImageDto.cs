
namespace Ecommerce.Service.src.DTO
{
    public class ImageReadDto
    {
        public Guid Id { get; set;}
        public required string Url { get; set; }
        public Guid ProductId { get; set; }
    }

    public class ImageCreateDto
    {
        public required string Url { get; set; }
        public Guid ProductId { get; set; }
    }

    public class ImageUpdateDto
    {
        public required string Url { get; set; }
    }
}
