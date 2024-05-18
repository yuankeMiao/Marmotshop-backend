using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IProductRepo
    {
        Task<QueryResult<Product>> GetAllProductsAsync(ProductQueryOptions? options);
        // Task<IEnumerable<Product>> GetMostPurchasedProductsAsync(int topNumber); // maybe implement it later
        Task<Product> GetProductByIdAsync(Guid productId);
        Task<Product> CreateProductWithTransactionAsync(Product newProduct);
        Task<Product> UpdateProductByIdAsync(Product updatedProduct);
        Task<bool> DeleteProductByIdAsync(Guid productId);
    }
}