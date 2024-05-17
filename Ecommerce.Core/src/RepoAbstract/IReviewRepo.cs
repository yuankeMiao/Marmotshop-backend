using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IReviewRepo
    {
        Task<QueryResult<Review>> GetAllReviewsAsync(ReviewQueryOptions? options);
        Task<QueryResult<Review>> GetAllReviewsByProductIdAsync(Guid productId, ReviewQueryOptions? options);
        Task<QueryResult<Review>> GetAllReviewsByUserIdAsync(Guid userId, ReviewQueryOptions? options);
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task<Review> CreateReviewAsync(Review newReview);
        Task<Review> UpdateReviewByIdAsync(Review updatedReview);
        Task<bool> DeleteReviewByIdAsync(Guid reviewId);
    }
}