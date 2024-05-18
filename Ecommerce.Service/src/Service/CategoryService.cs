using AutoMapper;

using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.Common;

using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.Shared;

namespace Ecommerce.Service.src.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;
        public CategoryService(IMapper mapper, ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CategoryReadDto>> GetAllCategoriesAsync()
        {
            var Categories = await _categoryRepo.GetAllCategoriesAsync();
            var CategoryReadDtos = Categories.Select(c => _mapper.Map<Category, CategoryReadDto>(c));
            return CategoryReadDtos;
        }
        public async Task<CategoryReadDto> GetCategoryByIdAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new Exception("bad request");
            }

            // if not found, repo will throw AppException.NotFound here
            var foundCategory = await _categoryRepo.GetCategoryByIdAsync(categoryId);
            var foundCategoryDto = _mapper.Map<Category, CategoryReadDto>(foundCategory);
            return foundCategoryDto;
        }
        public async Task<CategoryReadDto> CreateCategoryAsync(CategoryCreateDto categoryCreateDto)
        {
            // validations
            if (string.IsNullOrEmpty(categoryCreateDto.Name)) throw AppException.InvalidInput("Category name cannot be empty");
            if (categoryCreateDto.Name.Length > 20) throw AppException.InvalidInput("Category name cannot be longer than 20 characters");

            if (categoryCreateDto.Image is not null && !ValidationHelper.IsImageUrlValid(categoryCreateDto.Image))
            {
                throw AppException.InvalidInput("Image must be a url");
            }

            var newCategory = _mapper.Map<CategoryCreateDto, Category>(categoryCreateDto);
            var createdCategory = await _categoryRepo.CreateCategoryAsync(newCategory);

            var createdCategoryDto = _mapper.Map<Category, CategoryReadDto>(createdCategory);
            return createdCategoryDto;
        }
        public async Task<CategoryReadDto> UpdateCategoryByIdAsync(Guid categoryId, CategoryUpdateDto categoryUpdateDto)
        {
            var foundCategory = await _categoryRepo.GetCategoryByIdAsync(categoryId);

            // validations
            if (categoryUpdateDto.Name is not null && string.IsNullOrEmpty(categoryUpdateDto.Name)) throw AppException.InvalidInput("Category name cannot be empty");
            if (categoryUpdateDto.Name is not null && categoryUpdateDto.Name.Length > 20) throw AppException.InvalidInput("Category name cannot be longer than 20 characters");
            if (categoryUpdateDto.Image is not null && !ValidationHelper.IsImageUrlValid(categoryUpdateDto.Image)) throw AppException.InvalidInput("Image must be a url");

            foundCategory.Name = categoryUpdateDto.Name ?? foundCategory.Name;
            foundCategory.Image = categoryUpdateDto.Image ?? foundCategory.Image;

            foundCategory.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            var updateCategory = await _categoryRepo.UpdateCategoryByIdAsync(foundCategory);

            var updatedCategoryDto = _mapper.Map<Category, CategoryReadDto>(updateCategory);

            return updatedCategoryDto;
        }
        public async Task<bool> DeleteCategoryByIdAsync(Guid categoryId)
        {
            // if category not found, repo will throw AppException.NotFound
            if (categoryId == Guid.Empty) AppException.InvalidInput("Category id is required");

            await _categoryRepo.DeleteCategoryByIdAsync(categoryId);
            return true;
        }
    }
}