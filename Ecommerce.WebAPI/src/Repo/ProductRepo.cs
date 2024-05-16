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

        public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryOptions? options)
        {
            // var query = _products.AsQueryable();
            var query = _products.AsQueryable();
            query = query.Include(p => p.Images);
            // Apply filters if ProductQueryOptions is not null
            if (options != null)
            {
                // Filter by search title
                if (!string.IsNullOrEmpty(options.Title))
                {
                    var lowercaseTitle = options.Title.ToLower(); // Convert title to lowercase
                    query = query.Where(p => p.Title.ToLower().Contains(lowercaseTitle));
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

                // Sorting
                if (!string.IsNullOrEmpty(options.SortBy))
                {
                    query = options.SortBy.ToLower() switch
                    {
                        "title" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                        "price" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                        _ => options.SortOrder == "desc" ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),// Default sorting by created date if sort by is not specified or invalid
                    };
                }

                // Pagination
                query = query.Skip(options.Offset).Take(options.Limit);
            }

            // Execute the query
            var products = await query.ToListAsync();
            return products;
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
                if (foundProduct is not null) throw AppException.DuplicateException("Product Title already exist");

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
                // rewrite images, because image updates are more complicated in front end
                // so it is eaasier to just re-write them
                if (updatedProduct.Images is not null)
                {
                    // remove existing images
                    var foundImages = await _images.Where(i => i.ProductId == updatedProduct.Id).ToListAsync();
                    foreach (var image in foundImages)
                    {
                        _images.Remove(image);
                    }
                    await _context.SaveChangesAsync();
                    // add new images
                    foreach (var image in updatedProduct.Images)
                    {
                        await _images.AddAsync(image);
                    }
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return updatedProduct;
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

        // will modify later
        public async Task<IEnumerable<Product>> GetMostPurchasedProductsAsync(int topNumber)
        {
            var parameters = new List<object> { topNumber };

            var mostPurchasedProducts = await _products
                .FromSqlRaw("SELECT * FROM public.get_most_purchased_products({0})", parameters.ToArray())
                .ToListAsync();

            return mostPurchasedProducts;
        }

    }
}