using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.Common;
using Ecommerce.WebAPI.src.Database;
using Ecommerce.Core.src.RepoAbstract;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.WebAPI.src.Repo
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Product> _products;
        private readonly DbSet<Image> _images;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
            _products = _context.Products;
            _images = _context.Images;
        }

        public async Task<QueryResult<Product>> GetAllProductsAsync(ProductQueryOptions? options)
        {
            var query = _products.AsQueryable();
            query = query.Include(p => p.Images);

            if (options is not null)
            {
                // Filter by search title
                if (!string.IsNullOrEmpty(options.Title))
                {
                    var lowercaseTitle = options.Title.ToLower();
                    query = query.Where(p => p.Title.Contains(lowercaseTitle, StringComparison.CurrentCultureIgnoreCase));
                }

                // Filter by price range
                if (options.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= options.MinPrice.Value);
                }

                if (options.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= options.MaxPrice.Value);
                }

                // Filter by category ID
                if (options.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == options.CategoryId);
                }

                // filter by is in stock
                if (options.InStock.HasValue)
                {
                    query = query.Where(p => p.Stock > 0);
                }

                // Sorting
                if (options.SortBy is not null)
                {
                    query = options.SortBy switch
                    {
                        ProductSortByEnum.Title => options.SortOrder == SortOrderEnum.Desc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                        ProductSortByEnum.Price => options.SortOrder == SortOrderEnum.Desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                        ProductSortByEnum.Created_Date => options.SortOrder == SortOrderEnum.Desc ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
                        ProductSortByEnum.Updated_Date => options.SortOrder == SortOrderEnum.Desc ? query.OrderByDescending(p => p.UpdatedDate) : query.OrderBy(p => p.UpdatedDate),
                        _ => query.OrderBy(p => p.CreatedDate),
                    };
                }

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                // Pagination
                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }
                var products = await query.ToListAsync();
                return new QueryResult<Product> { Data = products, TotalCount = totalCount };
            }
            else
            {
                // If no query options provided, return all without pagination
                var products = await query.ToListAsync();
                return new QueryResult<Product> { Data = products, TotalCount = products.Count };
            }
        }

        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            var foundproduct = await _products.FindAsync(productId) ?? throw AppException.NotFound("Product not found");
            var foundImages = await _images.Where(i => i.ProductId == foundproduct.Id).ToListAsync();
            foundproduct.Images = foundImages;

            return foundproduct;
        }


        public async Task<Product> CreateProductWithTransactionAsync(Product newProduct)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // create product record
                var foundProduct = await _products.FirstOrDefaultAsync(p => p.Title == newProduct.Title);
                if (foundProduct is not null) throw AppException.Duplicate("Product Title");

                var createdProduct = await _products.AddAsync(newProduct);

                // create image record
                if (newProduct.Images is not null)
                {
                    foreach (var newImage in newProduct.Images)
                    {
                        newImage.ProductId = newProduct.Id;
                        var createdImage = await _images.AddAsync(newImage);
                    }
                }
                await _context.SaveChangesAsync();
                transaction.Commit();

                return newProduct;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Product> UpdateProductByIdAsync(Product updatedProduct)
        {

            _products.Update(updatedProduct);
            await _context.SaveChangesAsync();
            return updatedProduct;

        }

        public async Task<bool> DeleteProductByIdAsync(Guid productId)
        {
            var foundProduct = await _products.FindAsync(productId) ?? throw AppException.NotFound("product not found");

            _products.Remove(foundProduct);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}