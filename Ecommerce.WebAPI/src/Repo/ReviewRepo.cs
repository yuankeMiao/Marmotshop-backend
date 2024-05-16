using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Review> _reviews;

        public ReviewRepo(AppDbContext context)
        {
            _context = context;
            _reviews = _context.Reviews;
        }


        public async Task<IEnumerable<Review>> GetAllReviewsAsync(BaseQueryOptions? options)
        {
            var query = _reviews.AsQueryable();
            // Pagination
            if (options is not null)
            {
                query = query.OrderBy(r => r.CreatedDate)
                             .Skip(options.Offset)
                             .Take(options.Limit);
            }

            var reviews = await query.ToListAsync();
            return reviews;
        }

        // consider to add pagination here with more order options like orderby rating, only check review with content
        public async Task<IEnumerable<Review>> GetAllReviewsByProductIdAsync(Guid productId)
        {
            var query = _reviews.AsQueryable();
            query = query
                .Where(r => r.ProductId == productId)
                .OrderBy(r => r.CreatedDate);

            var reviews = await query.ToListAsync();
            return reviews;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsByUserIdAsync(Guid userId)
        {
            var query = _reviews.AsQueryable();
            query = query
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.CreatedDate);

            var reviews = await query.ToListAsync();
            return reviews;
        }

        public async Task<Review> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _reviews.FindAsync(reviewId) ?? throw AppException.NotFound("Review not found");
            return review;
        }

        public async Task<Review> CreateReviewAsync(Review newReview)
        {
            var review = await _reviews.AddAsync(newReview);
            await _context.SaveChangesAsync();

            return review.Entity;
        }

        public async Task<bool> DeleteReviewByIdAsync(Guid reviewId)
        {
            var foundReview = await _reviews.FindAsync(reviewId) ?? throw AppException.NotFound("Review not found");

            _reviews.Remove(foundReview);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Review> UpdateReviewByIdAsync(Review updatedReview)
        {
            _reviews.Update(updatedReview);
            await _context.SaveChangesAsync();
            
            return updatedReview;
        }
    }
}