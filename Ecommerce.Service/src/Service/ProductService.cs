
using System.Text.RegularExpressions;
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    // will modify later
    public class ProductService : IProductService
    {

        private readonly IProductRepo _productRepo;
        private IMapper _mapper;
        private readonly ICategoryRepo _categoryRepo;

        public ProductService(IProductRepo productRepo, IMapper mapper, ICategoryRepo categoryRepo)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllProductsAsync(ProductQueryOptions? productQueryOptions)
        {
            try
            {
                var products = await _productRepo.GetAllProductsAsync(productQueryOptions);
                var productDtos = _mapper.Map<IEnumerable<Product>, HashSet<ProductReadDto>>(products);

                return productDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ProductReadDto> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw AppException.InvalidInput("Product id cannot be null");
            }
            try
            {
                var product = await _productRepo.GetProductByIdAsync(productId);
                var productDto = _mapper.Map<ProductReadDto>(product);

                // var images = await _imageRepo.GetImagesByProductIdAsync(productDto.Id);
                productDto.Images = _mapper.Map<IEnumerable<ImageReadDto>>(productDto.Images);

                return productDto;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<ProductReadDto> CreateProductAsync(ProductCreateDto newProduct)
        {
            try
            {
                if (newProduct == null)
                {
                    throw new ArgumentNullException(nameof(newProduct), "Product cannot be null");
                }
                // Check if the product name is provided
                if (string.IsNullOrWhiteSpace(newProduct.Title))
                {
                    throw AppException.InvalidInput("Product name cannot be empty");
                }

                // Check if the price is greater than zero
                if (newProduct.Price <= 0)
                {
                    throw AppException.InvalidInput("Price should be greated than zero.");
                }

                // Validate image URLs
                if (newProduct.Images is not null)
                {

                    foreach (var image in newProduct.Images)
                    {
                        // Check if the URL is provided
                        if (string.IsNullOrWhiteSpace(image.Url))
                        {
                            throw AppException.InvalidInput("Image URL cannot be empty");
                        }

                        // Check if the URL points to a valid image format 
                        if (!IsImageUrlValid(image.Url))
                        {
                            throw AppException.InvalidInput("Invalid image format");
                        }
                    }
                }
                // Check if the specified category ID exists
                var category = await _categoryRepo.GetCategoryByIdAsync(newProduct.CategoryId) ?? throw AppException.NotFound("Category not found");

                var createdProduct = _mapper.Map<Product>(newProduct);
                createdProduct.Images = _mapper.Map<IEnumerable<Image>>(createdProduct.Images);

                await _productRepo.CreateProductWithTransactionAsync(createdProduct);
                var productReadDto = _mapper.Map<ProductReadDto>(createdProduct);
                productReadDto.Images = _mapper.Map<IEnumerable<ImageReadDto>>(productReadDto.Images);

                return productReadDto;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<ProductReadDto> UpdateProductByIdAsync(Guid productId, ProductUpdateDto productUpdateDto)
        {
            try
            {
                var foundProduct = await _productRepo.GetProductByIdAsync(productId);


                foundProduct.Title = productUpdateDto.Title ?? foundProduct.Title;
                foundProduct.Description = productUpdateDto.Description ?? foundProduct.Description;
                foundProduct.CategoryId = productUpdateDto.CategoryId ?? foundProduct.CategoryId;
                foundProduct.Price = productUpdateDto.Price ?? foundProduct.Price;
                foundProduct.DiscountPercentage = productUpdateDto.DiscountPercentage ?? foundProduct.DiscountPercentage;
                foundProduct.Stock = productUpdateDto.Stock ?? foundProduct.Stock;
                foundProduct.Brand = productUpdateDto.Brand ?? foundProduct.Brand;
                foundProduct.Thumbnail = productUpdateDto.Thumbnail ?? foundProduct.Thumbnail;
                foundProduct.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

                if (productUpdateDto.Images is not null)
                {
                    var updatedImages = _mapper.Map<IEnumerable<Image>>(productUpdateDto.Images);
                    foundProduct.Images = updatedImages;
                }

                var updatedProduct = await _productRepo.UpdateProductByIdWithTransactionAsync(foundProduct);

                var updatedProductReadDto = _mapper.Map<ProductReadDto>(updatedProduct);
                updatedProductReadDto.Images = _mapper.Map<IEnumerable<ImageReadDto>>(updatedProductReadDto.Images);
                
                return updatedProductReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new Exception("bad request");
            }
            try
            {
                return await _productRepo.DeleteProductByIdAsync(productId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Task<IEnumerable<ProductReadDto>> GetMostPurchasedProductsAsync(int topNumber)
        {
            throw new NotImplementedException();
        }

        // Method to validate image URL
        static bool IsImageUrlValid(string imageUrl)
        {
            // Regular expression pattern to match common image file extensions (e.g., .jpg, .jpeg, .png, .gif)
            string pattern = @"^(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|jpeg|gif|png)$";

            // Create a regular expression object
            Regex regex = new(pattern, RegexOptions.IgnoreCase);

            // Check if the URL matches the pattern
            return regex.IsMatch(imageUrl);
        }

    }
}