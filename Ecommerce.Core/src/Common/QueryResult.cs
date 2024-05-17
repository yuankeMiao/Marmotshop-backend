
namespace Ecommerce.Core.src.Common
{
    public class QueryResult<T>
    {
        public required IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}