
using AutoMapper;

using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;

using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.Shared;

namespace Ecommerce.Service.src.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private IMapper _mapper;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IImageRepo _imageRepo;

        public ProductService(IProductRepo productRepo, IMapper mapper, ICategoryRepo categoryRepo, IImageRepo imageRepo)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _imageRepo = imageRepo;
        }

        public async Task<QueryResult<ProductReadDto>> GetAllProductsAsync(ProductQueryOptions? productQueryOptions)
        {
            var queryResult = await _productRepo.GetAllProductsAsync(productQueryOptions);
            var products = queryResult.Data;
            var totalCount = queryResult.TotalCount;
            var productReadDtos = _mapper.Map<IEnumerable<ProductReadDto>>(products);

            return new QueryResult<ProductReadDto> { Data = productReadDtos, TotalCount = totalCount };
        }


        public async Task<ProductReadDto> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw AppException.InvalidInput("Product id cannot be null");
            }
            var product = await _productRepo.GetProductByIdAsync(productId);
            var productDto = _mapper.Map<ProductReadDto>(product);

            return productDto;
        }


        public async Task<ProductReadDto> CreateProductAsync(ProductCreateDto newProduct)
        {
            if (newProduct == null) throw new ArgumentNullException(nameof(newProduct), "Product cannot be null");

            if (string.IsNullOrWhiteSpace(newProduct.Title)) throw AppException.InvalidInput("Product name cannot be empty");
            if (string.IsNullOrWhiteSpace(newProduct.Description)) throw AppException.InvalidInput("Product description cannot be empty");

            // Check if category exists
            _ = await _categoryRepo.GetCategoryByIdAsync(newProduct.CategoryId) ?? throw AppException.NotFound("Category not found");


            if (newProduct.Price <= 0) throw AppException.InvalidInput("Price should be greated than zero.");
            if (newProduct.DiscountPercentage < 0) throw AppException.InvalidInput("Discount percentage should be greated than or equal to zero.");
            if (newProduct.Stock < 0) throw AppException.InvalidInput("Stock should be greated than or equal to zero.");
            if (!ValidationHelper.IsImageUrlValid(newProduct.Thumbnail)) throw AppException.InvalidInput("Thumbnail cannot be empty");
            if (newProduct.Images is not null)
            {
                foreach (var image in newProduct.Images)
                {
                    if (image is not null && !ValidationHelper.IsImageUrlValid(image.Url)) throw AppException.InvalidInput("Image must be a url");
                }
            }

            var createdProduct = _mapper.Map<Product>(newProduct);

            await _productRepo.CreateProductWithTransactionAsync(createdProduct);
            var productReadDto = _mapper.Map<ProductReadDto>(createdProduct);

            return productReadDto;
        }


        public async Task<ProductReadDto> UpdateProductByIdAsync(Guid productId, ProductUpdateDto productUpdateDto)
        {
            var foundProduct = await _productRepo.GetProductByIdAsync(productId);

            if (productUpdateDto.Title is not null)
            {
                if (string.IsNullOrWhiteSpace(productUpdateDto.Title)) throw AppException.InvalidInput("Product name cannot be empty");
                foundProduct.Title = productUpdateDto.Title;
            }

            if (productUpdateDto.Description is not null)
            {
                if (string.IsNullOrWhiteSpace(productUpdateDto.Description)) throw AppException.InvalidInput("Product description cannot be empty");
                foundProduct.Description = productUpdateDto.Description;
            }

            if (productUpdateDto.CategoryId.HasValue)
            {
                _ = await _categoryRepo.GetCategoryByIdAsync(productUpdateDto.CategoryId.Value) ?? throw AppException.NotFound("Category not found");
                foundProduct.CategoryId = productUpdateDto.CategoryId.Value;
            }

            if (productUpdateDto.Price.HasValue)
            {
                if (productUpdateDto.Price <= 0) throw AppException.InvalidInput("Price must be greater than zero");
                foundProduct.Price = productUpdateDto.Price.Value;
            }

            if (productUpdateDto.DiscountPercentage.HasValue)
            {
                if (productUpdateDto.DiscountPercentage < 0) throw AppException.InvalidInput("Discount percentage cannot be smaller than zero");
                foundProduct.DiscountPercentage = productUpdateDto.DiscountPercentage.Value;
            }

            if (productUpdateDto.Stock.HasValue)
            {
                if (productUpdateDto.Stock < 0) throw AppException.InvalidInput("Stock cannot be smaller than zero");
                foundProduct.Stock = productUpdateDto.Stock.Value;
            }

            if (productUpdateDto.Brand is not null)
            {
                if (string.IsNullOrWhiteSpace(productUpdateDto.Brand)) foundProduct.Brand = productUpdateDto.Brand;
            }

            if (productUpdateDto.Thumbnail is not null)
            {
                if (!ValidationHelper.IsImageUrlValid(productUpdateDto.Thumbnail)) throw AppException.InvalidInput("Thumbnail cannot be empty");
                foundProduct.Thumbnail = productUpdateDto.Thumbnail;
            }


            if (productUpdateDto.Images is not null)
            {
                var newImages = new List<Image>();
                foreach (var imageUpdateDto in productUpdateDto.Images)
                {
                    if (!ValidationHelper.IsImageUrlValid(imageUpdateDto.Url)) throw AppException.InvalidInput("Image must be a url");
                    var updatedImage = new Image
                    {
                        Id = Guid.NewGuid(),
                        Url = imageUpdateDto.Url,
                        ProductId = foundProduct.Id
                    };
                    newImages.Add(updatedImage);
                }
                var updatedImages = await _imageRepo.UpdateImagesAsync(foundProduct.Id, newImages);
            }


            foundProduct.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _productRepo.UpdateProductByIdAsync(foundProduct);
            var updatedProduct = await _productRepo.GetProductByIdAsync(productId);

            var updatedProductReadDto = _mapper.Map<ProductReadDto>(updatedProduct);
            return updatedProductReadDto;
        }


        public async Task<bool> DeleteProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty) throw AppException.InvalidInput("Product id is required");
            
            return await _productRepo.DeleteProductByIdAsync(productId);
        }
    }
}