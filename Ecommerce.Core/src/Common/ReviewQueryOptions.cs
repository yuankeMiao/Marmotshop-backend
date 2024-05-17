

namespace Ecommerce.Core.src.Common
{
    public class ReviewQueryOptions: BaseQueryOptions
    {
        public int? Rating { get; set; }
        public bool? Has_Content { get; set; }
    }
}