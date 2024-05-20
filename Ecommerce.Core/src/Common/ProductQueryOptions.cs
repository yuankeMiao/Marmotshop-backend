using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Common
{
    public class ProductQueryOptions : BaseQueryOptions
    {
        public string? Title { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? InStock { get; set; }
        public ProductSortByEnum? SortBy { get; set; }
    }
}