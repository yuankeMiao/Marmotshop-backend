using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.Common;
using Ecommerce.WebAPI.src.Database;
using Ecommerce.Core.src.RepoAbstract;
using Microsoft.EntityFrameworkCore;

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
                if (options.Min_Price.HasValue)
                {
                    query = query.Where(p => p.Price >= options.Min_Price.Value);
                }

                if (options.Max_Price.HasValue)
                {
                    query = query.Where(p => p.Price <= options.Max_Price.Value);
                }

                // Filter by category ID
                if (options.Category_Id.HasValue)
                {
                    query = query.Where(p => p.CategoryId == options.Category_Id);
                }

                // filter by is in stock
                if (options.In_Stock.HasValue)
                {
                    query = query.Where(p => p.Stock > 0);
                }

                // Sorting
                if (!string.IsNullOrEmpty(options.SortBy))
                {
                    query = options.SortBy.ToLower() switch
                    {
                        "title" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                        "price" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                        "created_date" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
                        "updated_date" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.UpdatedDate) : query.OrderBy(p => p.UpdatedDate),
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

                await _products.AddAsync(newProduct);

                // create image record
                if (newProduct.Images is not null)
                {
                    foreach (var newImage in newProduct.Images)
                    {
                        await _images.AddAsync(newImage);
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

        public async Task<Product> UpdateProductByIdWithTransactionAsync(Product updatedProduct)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                //check if product exist
                var foundProduct = await _products.FindAsync(updatedProduct.Id) ?? throw AppException.NotFound("Product not found");
                // update product table
                _products.Update(updatedProduct);
                /* rewrite images, because image updates are more complicated in front end
                one single update of product might contrains multiple uodates on images and delete image, create new images...
                so it is eaasier to just re-write them
                even thought it will couse more queries, but since I don't expect client side will update products very frequently
                so I will use this solution for now */

                if (updatedProduct.Images is not null && updatedProduct.Images.Any())
                {
                    // remove existing images
                    var foundImages = await _images.Where(i => i.ProductId == updatedProduct.Id).ToListAsync();
                    foreach (var image in foundImages)
                    {
                        _images.RemoveRange(_images.Where(i => i.ProductId == updatedProduct.Id));
                    }
                    // add new images
                    foreach (var image in updatedProduct.Images)
                    {
                        image.Id = Guid.NewGuid();
                        await _images.AddRangeAsync(updatedProduct.Images);
                    }
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return _products.FirstOrDefault(p => p.Id == updatedProduct.Id) ?? throw AppException.NotFound("Product not found");
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
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