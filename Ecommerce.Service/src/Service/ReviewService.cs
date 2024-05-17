using System.Threading.Channels;
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
        private IOrderRepo _orderRepo;

        public ReviewService(IReviewRepo repo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo, IOrderRepo orderRepo)
        {
            _mapper = mapper;
            _reviewRepo = repo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _orderRepo = orderRepo;
        }

        public async Task<QueryResult<ReviewReadDto>> GetAllReviewsAsync(ReviewQueryOptions? options)
        {
            try
            {
                var queryResult = await _reviewRepo.GetAllReviewsAsync(options);
                var reviews = queryResult.Data;
                var totalCount = queryResult.TotalCount;

                var reviewReadDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

                return new QueryResult<ReviewReadDto> { Data = reviewReadDtos, TotalCount = totalCount };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QueryResult<ReviewReadDto>> GetAllReviewsByProductIdAsync(Guid productId, ReviewQueryOptions? options)
        {
            try
            {
                _ = await _productRepo.GetProductByIdAsync(productId) ?? throw AppException.NotFound("Product not found");
                var queryResult = await _reviewRepo.GetAllReviewsByProductIdAsync(productId, options);
                var reviews = queryResult.Data;
                var totalCount = queryResult.TotalCount;

                var reviewReadDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

                return new QueryResult<ReviewReadDto> { Data = reviewReadDtos, TotalCount = totalCount };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QueryResult<ReviewReadDto>> GetAllReviewsByUserIdAsync(Guid userId, ReviewQueryOptions? options)
        {
            try
            {
                _ = await _userRepo.GetUserByIdAsync(userId) ?? throw AppException.NotFound("User not found");
                var queryResult = await _reviewRepo.GetAllReviewsByUserIdAsync(userId, options);
                var reviews = queryResult.Data;
                var totalCount = queryResult.TotalCount;

                var reviewReadDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

                return new QueryResult<ReviewReadDto> { Data = reviewReadDtos, TotalCount = totalCount };
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


        public async Task<ReviewReadDto> CreateReviewAsync(Guid userId, ReviewCreateDto reviewCreateDto)
        {
            // check if user and product exist
            _ = await _userRepo.GetUserByIdAsync(userId);
            _ = await _productRepo.GetProductByIdAsync(reviewCreateDto.ProductId);

            //check if the user ever bought the product

            var orderResult = await _orderRepo.GetAllOrdersByUserIdAsync(userId, null);
            var foundOrders = orderResult.Data;
            bool hasPerchased = foundOrders.Any(order => order.Products.Any(product => product.ProductId == reviewCreateDto.ProductId));
            if (!hasPerchased) throw AppException.InvalidInput("User cannot give review to product not purchased");

            // validation
            if (reviewCreateDto.Rating < 1 || reviewCreateDto.Rating > 5) throw AppException.InvalidInput("Raiting should be from 1 to 5");

            var newReview = _mapper.Map<Review>(reviewCreateDto);
            newReview.UserId = userId;
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