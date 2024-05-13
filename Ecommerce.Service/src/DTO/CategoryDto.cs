using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class CategoryReadDto: BaseEntity
    {
        public required string CategoryName { get; set; }
        public required string CategoryImage { get; set; }
    }

    public class CategoryCreateDto
    {
        public required string CategoryName { get; set; }
        public required string CategoryImage { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string? CategoryName { get; set; }
        public string? CategoryImage { get; set; }
    }
}