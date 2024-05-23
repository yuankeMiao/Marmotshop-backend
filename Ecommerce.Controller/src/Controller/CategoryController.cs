using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAllCategoriesAsync() // endpoint: /categories
        {
            var categories = await _categoryService.GetAllCategoriesAsync(); // exception middleware will handle exceptions in service layer
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}")] // endpoint: /categories/:category_id
        public async Task<ActionResult<CategoryReadDto>> GetCategoryByIdAsync([FromRoute] Guid categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost] // endpoint: /categories
        public async Task<ActionResult<CategoryReadDto>> CreateCategoryAsync([FromBody] CategoryCreateDto categoryCreateDto)
        {
            var category = await _categoryService.CreateCategoryAsync(categoryCreateDto);
            // CreatedAtAction is not working, so i set the url manually for now, might check later if i have time
            return StatusCode(201, category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryId}")] // endpoint: /categories/:category_id
        public async Task<ActionResult<CategoryReadDto>> UpdateCategoryByIdAsync([FromRoute] Guid categoryId, [FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            var category = await _categoryService.UpdateCategoryByIdAsync(categoryId, categoryUpdateDto);
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId}")] // endpoint: /categories/:category_id
        public async Task<ActionResult<bool>> DeleteCategoryByIdAsync([FromRoute] Guid categoryId)
        {
            var deleted = await _categoryService.DeleteCategoryByIdAsync(categoryId); //if deletion is failed, server layer will throw an exception and handled by middleware
            return Ok(deleted);
        }
    }
}