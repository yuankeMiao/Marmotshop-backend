using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Common
{
    public class ProductQueryOptions : BaseQueryOptions
    {
        public string? Title { get; set; }
        public decimal? Min_Price { get; set; }
        public decimal? Max_Price { get; set; }
        public Guid? Category_Id { get; set; }
        public bool? In_Stock { get; set; }
        public ProductSortByEnum? SortBy { get; set; }
    }
}