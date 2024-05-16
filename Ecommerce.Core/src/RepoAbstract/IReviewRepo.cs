using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IReviewRepo
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync(BaseQueryOptions options);
        Task<IEnumerable<Review>> GetAllReviewsByProductIdAsync(Guid productId);
        Task<IEnumerable<Review>> GetAllReviewsByUserIdAsync(Guid userId);
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task<Review> CreateReviewAsync(Review newReview);
        Task<Review> UpdateReviewByIdAsync(Review updatedReview);
        Task<bool> DeleteReviewByIdAsync(Guid reviewId);
    }
}