using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IReviewService
    {
        Task<QueryResult<ReviewReadDto>> GetAllReviewsAsync(ReviewQueryOptions? options);
        Task<QueryResult<ReviewReadDto>> GetAllReviewsByProductIdAsync(Guid productId, ReviewQueryOptions? options);
        Task<QueryResult<ReviewReadDto>> GetAllReviewsByUserIdAsync(Guid UserId, ReviewQueryOptions? options);
        Task<ReviewReadDto> GetReviewByIdAsync(Guid reviewId);
        Task<ReviewReadDto> CreateReviewAsync(Guid userId, ReviewCreateDto reviewCreateDto);
        Task<ReviewReadDto> UpdateReviewByIdAsync(Guid reviewId, ReviewUpdateDto reviewUpdateDto);
        Task<bool> DeleteReviewByIdAsync(Guid reviewId);
    }
}