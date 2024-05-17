using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductsAsync([FromQuery] ProductQueryOptions? options)
        {

            var products = await _productService.GetAllProductsAsync(options);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductReadDto>> GetProductByIdAsync([FromRoute] Guid productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost()]
        public async Task<ActionResult<ProductReadDto>> CreateProductAsync([FromBody] ProductCreateDto productCreateDto)
        {

            var product = await _productService.CreateProductAsync(productCreateDto);
            return Created($"http://localhost:5227/api/v1/products/{product.Id}", product);

        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{productId}")]
        public async Task<ActionResult<ProductReadDto>> UpdateProductByIdAsync([FromRoute] Guid productId, [FromBody] ProductUpdateDto productUpdateDto)
        {

            var product = await _productService.UpdateProductByIdAsync(productId, productUpdateDto);
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId}")]
        public async Task<ActionResult<bool>> DeleteProductByIdAsync([FromRoute] Guid productId)
        {
            var deleted = await _productService.DeleteProductByIdAsync(productId);
            return Ok(deleted);
        }

    }
}