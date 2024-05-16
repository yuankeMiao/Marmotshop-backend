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

        public async Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto reviewCreateDto)
        {
            var foundUser = await _userRepo.GetUserByIdAsync(reviewCreateDto.UserId);
            if (foundUser is null)
            {
                throw AppException.NotFound("User not found");
            }

            var foundProduct = await _productRepo.GetProductByIdAsync(reviewCreateDto.ProductId);
            if (foundProduct is null)
            {
                throw AppException.NotFound("Product not found");
            }

            // Create a new Review object and set its properties
            var review = new Review
            {
                Rating = reviewCreateDto.Rating,
                Content = reviewCreateDto.Content,
                UserId = reviewCreateDto.UserId,
                User = foundUser,
                ProductId = reviewCreateDto.ProductId,
                Product = foundProduct
            };

            var createdReview = await _reviewRepo.CreateReviewAsync(review);

            var reviewReadDto = _mapper.Map<ReviewReadDto>(createdReview);

            return reviewReadDto;
        }

        public async Task<bool> DeleteReviewByIdAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw new Exception("bad request");
            }
            var isDeleted = await _reviewRepo.DeleteReviewByIdAsync(reviewId);

            return isDeleted;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsAsync(BaseQueryOptions options)
        {
            var reviews = await _reviewRepo.GetAllReviewsAsync(options);
            var reviewDtos = new List<ReviewReadDto>();

            foreach (var review in reviews)
            {
                reviewDtos.Add(_mapper.Map<ReviewReadDto>(review));
            }

            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsByProductIdAsync(Guid productId)
        {
            var foundProduct = _productRepo.GetProductByIdAsync(productId);
            if (foundProduct is null)
            {
                throw AppException.NotFound("Product not found");
            }
            var result = await _reviewRepo.GetAllReviewsByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<Review>, IEnumerable<ReviewReadDto>>(result);
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ReviewReadDto> GetReviewByIdAsync(Guid reviewId)
        {
            var foundReview = await _reviewRepo.GetReviewByIdAsync(reviewId);
            if (foundReview is null)
            {
                throw AppException.NotFound("Review not found");
            }
            var reviewReadDto = _mapper.Map<ReviewReadDto>(foundReview);
            return reviewReadDto;
        }

        public async Task<ReviewReadDto> UpdateReviewByIdAsync(Guid reviewId, ReviewUpdateDto reviewUpdateDto)
        {
            var foundReview = await _reviewRepo.GetReviewByIdAsync(reviewId);
            if (foundReview is null)
            {
                throw AppException.NotFound("Review not found");
            }

            // Update
            if (reviewUpdateDto.Rating != null)
            {
                foundReview.Rating = reviewUpdateDto.Rating.Value;
            }
            if (reviewUpdateDto.Content != null)
            {
                foundReview.Content = reviewUpdateDto.Content;
            }
            foundReview.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            // Save changes
            var updatedReview = await _reviewRepo.UpdateReviewByIdAsync(foundReview);

            var reviewReadDto = _mapper.Map<ReviewReadDto>(updatedReview);

            return reviewReadDto;
        }
    }
}