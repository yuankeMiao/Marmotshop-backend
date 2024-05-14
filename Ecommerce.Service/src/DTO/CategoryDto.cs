using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class CategoryReadDto: BaseEntity
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
    }

    public class CategoryCreateDto
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
    }
}