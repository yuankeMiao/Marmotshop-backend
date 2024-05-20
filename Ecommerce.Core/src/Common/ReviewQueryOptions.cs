

using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Common
{
    public class ReviewQueryOptions: BaseQueryOptions
    {
        public int? Rating { get; set; }
        public bool? Has_Content { get; set; }
        public ReviewSortByEnum? SortBy { get; set; } = null;
    }
}