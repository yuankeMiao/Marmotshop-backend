using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Common
{
    public class BaseQueryOptions
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 20;
        public SortOrderEnum? SortOrder { get; set; } = SortOrderEnum.Asc;
    }
}