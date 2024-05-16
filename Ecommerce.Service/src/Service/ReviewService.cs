using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class ReviewService : IReviewService
    {
        private IMapper _mapper;
        private readonly IReviewRepo _reviewRepo;
        private IUserRepo _userRepo;
        private IProductRepo _productRepo;

        public ReviewService(IReviewRepo repo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo)
        {
            _mapper = mapper;
            _reviewRepo = repo;
            _productRepo = productRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsAsync(BaseQueryOptions? options)
        {
            try
            {
                var reviews = await _reviewRepo.GetAllReviewsAsync(options);
                var reviewReadDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

                return reviewReadDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsByProductIdAsync(Guid productId)
        {
            try
            {
                _ = _productRepo.GetProductByIdAsync(productId) ?? throw AppException.NotFound("Product not found");
                var reviews = await _reviewRepo.GetAllReviewsByProductIdAsync(productId);
                var reviewReadDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

                return reviewReadDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsByUserIdAsync(Guid userId)
        {
            try
            {
                _ = _userRepo.GetUserByIdAsync(userId) ?? throw AppException.NotFound("User not found");
                var reviews = await _reviewRepo.GetAllReviewsByProductIdAsync(userId);
                var reviewReadDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

                return reviewReadDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ReviewReadDto> GetReviewByIdAsync(Guid reviewId)
        {
            var foundReview = await _reviewRepo.GetReviewByIdAsync(reviewId) ?? throw AppException.NotFound("Review not found");
            
            var reviewReadDto = _mapper.Map<ReviewReadDto>(foundReview);
            return reviewReadDto;
        }


        public async Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto reviewCreateDto)
        {
            // check if user and product exist
            _ = await _userRepo.GetUserByIdAsync(reviewCreateDto.UserId) ?? throw AppException.NotFound("User not found");
            _ = await _productRepo.GetProductByIdAsync(reviewCreateDto.ProductId) ?? throw AppException.NotFound("Product not found");

            // validation
            if (reviewCreateDto.Rating < 1 || reviewCreateDto.Rating > 5) throw AppException.InvalidInput("Raiting should be from 1 to 5");

            var newReview = _mapper.Map<Review>(reviewCreateDto);
            var createdReview = await _reviewRepo.CreateReviewAsync(newReview);

            var reviewReadDto = _mapper.Map<ReviewReadDto>(createdReview);

            return reviewReadDto;
        }

        public async Task<ReviewReadDto> UpdateReviewByIdAsync(Guid reviewId, ReviewUpdateDto reviewUpdateDto)
        {
            // check if review exists
            var foundReview = await _reviewRepo.GetReviewByIdAsync(reviewId) ?? throw AppException.NotFound("Review not found");

            // Update
            foundReview.Rating = reviewUpdateDto.Rating ?? foundReview.Rating;
            foundReview.Content = reviewUpdateDto.Content ?? foundReview.Content;
            foundReview.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            // Save changes
            var updatedReview = await _reviewRepo.UpdateReviewByIdAsync(foundReview);
            var reviewReadDto = _mapper.Map<ReviewReadDto>(updatedReview);

            return reviewReadDto;
        }

        public async Task<bool> DeleteReviewByIdAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw AppException.InvalidInput("Review Id should not be empty");
            }
            var isDeleted = await _reviewRepo.DeleteReviewByIdAsync(reviewId);

            return isDeleted;
        }

    }
}