
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
        private readonly IProductImageRepo _productImageRepo;

        public ProductService(IProductRepo productRepo, IMapper mapper, ICategoryRepo categoryRepo, IProductImageRepo productImageRepo)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _productImageRepo = productImageRepo;
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllProductsAsync(ProductQueryOptions? productQueryOptions)
        {

            try
            {
                var products = await _productRepo.GetAllProductsAsync(productQueryOptions);
                var productDtos = _mapper.Map<HashSet<ProductReadDto>>(products);


                foreach (var productDto in productDtos)
                {
                    // Fetch category information for the product
                    var category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId);
                    var categoryDto = _mapper.Map<CategoryReadDto>(category);

                    // Set the category property in the product DTO
                    productDto.Category = categoryDto;
                }

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
                throw AppException.BadRequest();
            }
            try
            {

                var product = await _productRepo.GetProductByIdAsync(productId);
                var productDto = _mapper.Map<ProductReadDto>(product);

                // Fetch category information for the product
                var category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId);
                var categoryDto = _mapper.Map<CategoryReadDto>(category);

                // Set the category property in the product DTO
                productDto.Category = categoryDto;
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
                    throw new ArgumentNullException(nameof(newProduct), "ProductC cannot be null");
                }
                // Check if the product name is provided
                if (string.IsNullOrWhiteSpace(newProduct.Title))
                {
                    throw AppException.InvalidInputException("Product name cannot be empty");
                }

                // Check if the price is greater than zero
                if (newProduct.Price <= 0)
                {
                    throw AppException.InvalidInputException("Price should be greated than zero.");
                }

                // Validate image URLs
                if (newProduct.ImageUrls is not null)
                {

                    foreach (var imageUrl in newProduct.ImageUrls)
                    {
                        // Check if the URL is provided
                        if (string.IsNullOrWhiteSpace(imageUrl))
                        {
                            throw AppException.InvalidInputException("Image URL cannot be empty");
                        }

                        // Check if the URL points to a valid image format 
                        if (!IsImageUrlValid(imageUrl))
                        {
                            throw AppException.InvalidInputException("Invalid image format");
                        }
                    }
                }
                // Check if the specified category ID exists
                var category = await _categoryRepo.GetCategoryByIdAsync(newProduct.CategoryId);
                if (category == null)
                {
                    throw AppException.NotFound("Category not found");
                }
                var productEntity = _mapper.Map<Product>(newProduct);
                var createdProduct = await _productRepo.CreateProductAsync(productEntity);

                var productReadDto = _mapper.Map<ProductReadDto>(createdProduct);
                productReadDto.Category = _mapper.Map<CategoryReadDto>(category);

                return productReadDto;
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
                var deletedProduct = await _productRepo.DeleteProductByIdAsync(productId);

                if (!deletedProduct)
                {
                    return false;
                }
                else
                {
                    return true;
                }
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


        public async Task<ProductReadDto> UpdateProductByIdAsync(Guid productId, ProductUpdateDto productUpdateDto)
        {
            try
            {
                var foundProduct = await _productRepo.GetProductByIdAsync(productId);


                foundProduct.Title = productUpdateDto.Title ?? foundProduct.Title;
                foundProduct.Description = productUpdateDto.Description ?? foundProduct.Description;
                foundProduct.CategoryId = productUpdateDto.CategoryId ?? foundProduct.CategoryId;

                foundProduct.Price = productUpdateDto.Price ?? foundProduct.Price;
                foundProduct.Stock = productUpdateDto.Stock ?? foundProduct.Stock;

                foundProduct.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

                // Find product images
                var productImages = await _productImageRepo.GetProductImagesByProductIdAsync(productId);


                // Update product images
                if (productUpdateDto.ImageUrls is not null && productUpdateDto.ImageUrls.Any())
                {
                    foreach (var imageUrl in productUpdateDto.ImageUrls)
                    {
                        // Find the image to update by URL
                        var imageToUpdate = productImages.FirstOrDefault(img => img.Url == imageUrl);

                        if (imageToUpdate is not null)
                        {
                            // Update image URL if it has changed
                            if (imageToUpdate.Url != imageUrl)
                            {
                                // Update the image URL using the repository method
                                var updateResult = _productImageRepo.UpdateImageUrlAsync(imageToUpdate.Id, imageUrl);
                            }
                        }
                        else
                        {
                            // Handle the case where the image URL from the DTO doesn't match any existing images
                            throw new Exception($"Image with URL {imageUrl} not found.");
                        }

                        // Validate image URL
                        if (!IsImageUrlValid(imageUrl))
                        {
                            throw AppException.InvalidInputException("Invalid image URL format");
                        }
                    }
                }


                // Save changes to the database
                var updatedProduct = await _productRepo.UpdateProductByIdAsync(foundProduct);

                // Fetch category information for the updated product
                var category = await _categoryRepo.GetCategoryByIdAsync(updatedProduct.CategoryId);
                var categoryDto = _mapper.Map<CategoryReadDto>(category);
                // Map the updated product entity to ProductReadDto
                var updatedProductDto = _mapper.Map<Product, ProductReadDto>(updatedProduct);
                // Update the productInventory value in the returned DTO
                updatedProductDto.Stock= foundProduct.Stock;
                // Set the category property in the updated product DTO
                updatedProductDto.Category = categoryDto;
                return updatedProductDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Method to validate image URL
        bool IsImageUrlValid(string imageUrl)
        {
            // Regular expression pattern to match common image file extensions (e.g., .jpg, .jpeg, .png, .gif)
            string pattern = @"^(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|jpeg|gif|png)$";

            // Create a regular expression object
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Check if the URL matches the pattern
            return regex.IsMatch(imageUrl);
        }

    }
}