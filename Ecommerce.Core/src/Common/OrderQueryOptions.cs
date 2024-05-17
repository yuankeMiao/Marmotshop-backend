
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Core.src.Common
{
    public class OrderQueryOptions: BaseQueryOptions
    {
        public OrderStatus? Status{ get; set; }
    }
}