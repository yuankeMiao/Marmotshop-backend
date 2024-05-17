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


        public async Task<QueryResult<Review>> GetAllReviewsAsync(ReviewQueryOptions? options)
        {
            var query = _reviews.AsQueryable();
            if (options is not null)
            {
                ApplyQueryOptions(query, options);

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                // Pagination
                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }
                var reviews = await query.ToListAsync();
                return new QueryResult<Review> { Data = reviews, TotalCount = totalCount };
            }
            else
            {
                var reviews = await query.ToListAsync();
                return new QueryResult<Review> { Data = reviews, TotalCount = reviews.Count };
            }
        }

        // consider to add pagination here with more order options like orderby rating, only check review with content
        public async Task<QueryResult<Review>> GetAllReviewsByProductIdAsync(Guid productId, ReviewQueryOptions? options)
        {
            var query = _reviews.AsQueryable();
            query = query.Where(r => r.ProductId == productId);

            if (options is not null)
            {
                ApplyQueryOptions(query, options);

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                // Pagination
                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }
                var reviews = await query.ToListAsync();
                return new QueryResult<Review> { Data = reviews, TotalCount = totalCount };
            }
            else
            {
                var reviews = await query.ToListAsync();
                return new QueryResult<Review> { Data = reviews, TotalCount = reviews.Count };
            }
        }

        public async Task<QueryResult<Review>> GetAllReviewsByUserIdAsync(Guid userId, ReviewQueryOptions? options)
        {
            var query = _reviews.AsQueryable();
            query = query.Where(r => r.UserId == userId);

            if (options is not null)
            {
                ApplyQueryOptions(query, options);

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                // Pagination
                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }
                var reviews = await query.ToListAsync();
                return new QueryResult<Review> { Data = reviews, TotalCount = totalCount };
            }
            else
            {
                var reviews = await query.ToListAsync();
                return new QueryResult<Review> { Data = reviews, TotalCount = reviews.Count };
            }
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

        private static IQueryable<Review> ApplyQueryOptions(IQueryable<Review> query, ReviewQueryOptions options)
        {
            if (options.Rating is not null)
            {
                query = query.Where(r => r.Rating == options.Rating);
            }

            if (options.Has_Content is not null)
            {
                query = query.Where(r => !string.IsNullOrEmpty(r.Content));
            }

            // Sorting
            if (!string.IsNullOrEmpty(options.SortBy))
            {
                query = options.SortBy.ToLower() switch
                {
                    "created_date" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
                    "updated_date" => options.SortOrder == "desc" ? query.OrderByDescending(p => p.UpdatedDate) : query.OrderBy(p => p.UpdatedDate),
                    _ => query.OrderBy(p => p.CreatedDate),
                };
            }

            return query;
        }

    }
}