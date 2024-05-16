using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewReadDto>> GetAllReviewsAsync(BaseQueryOptions options);
        Task<IEnumerable<ReviewReadDto>> GetAllReviewsByProductIdAsync(Guid productId);
        Task<IEnumerable<ReviewReadDto>> GetAllReviewsByUserIdAsync(Guid UserId);
        Task<ReviewReadDto> GetReviewByIdAsync(Guid reviewId);
        Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto reviewCreateDto);
        Task<ReviewReadDto> UpdateReviewByIdAsync(Guid reviewId, ReviewUpdateDto reviewUpdateDto);
        Task<bool> DeleteReviewByIdAsync(Guid reviewId);
    }
}