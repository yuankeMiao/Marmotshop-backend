using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/reviews")]
    public class ReviewController : ControllerBase
    {
        private IReviewService _service;
        private IAuthorizationService _authorizationService;
        public ReviewController(IReviewService service, IAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetAllReviewsAsync([FromQuery] ReviewQueryOptions options)
        {
            var result = await _service.GetAllReviewsAsync(options);
            var reviews = result.Data;
            var totalCount = result.TotalCount;

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetAllReviewsByProductIdAsync([FromRoute] Guid productId, [FromQuery] ReviewQueryOptions options)
        {
            var result = await _service.GetAllReviewsByProductIdAsync(productId, options);
            var reviews = result.Data;
            var totalCount = result.TotalCount;

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetAllReviewsByUserIdAsync([FromRoute] Guid userId, [FromQuery] ReviewQueryOptions options)
        {
            var result = await _service.GetAllReviewsByUserIdAsync(userId, options);
            var reviews = result.Data;
            var totalCount = result.TotalCount;

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("my-reviews")]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetMyReviewsAsync([FromQuery] ReviewQueryOptions options)
        {
            var userId = GetUserIdClaim();
            var result = await _service.GetAllReviewsByUserIdAsync(userId, options);
            var reviews = result.Data;
            var totalCount = result.TotalCount;

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpGet("{reviewId}")]
        public async Task<ActionResult<ReviewReadDto>> GetReviewByIdAsync([FromRoute] Guid reviewId)
        {
            var review = await _service.GetReviewByIdAsync(reviewId);
            return Ok(review);
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<ReviewReadDto>> CreateReviewAsync([FromBody] ReviewCreateDto reviewCreateDto)
        {
            var userId = GetUserIdClaim();
            var review = await _service.CreateReviewAsync(userId, reviewCreateDto);
            return Created($"http://localhost:5227/api/v1/reviews/{review.Id}", review);
        }

        [Authorize]
        [HttpPatch("{reviewId}")]
        public async Task<ActionResult<ReviewReadDto>> UpdateReviewByIdAsync([FromRoute] Guid reviewId, [FromBody] ReviewUpdateDto reviewUpdateDto)
        {
            var review = await _service.GetReviewByIdAsync(reviewId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, review, "AdminOrOwnerReview");

            if (authResult.Succeeded)
            {
                var updatedReview = await _service.UpdateReviewByIdAsync(reviewId, reviewUpdateDto);
                return Ok(updatedReview);
            }
            else
            {
                return Forbid();
            }
        }

        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<bool>> DeleteReviewByIdAsync([FromRoute] Guid reviewId)
        {
            var review = await _service.GetReviewByIdAsync(reviewId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, review, "AdminOrOwnerReview");

            if (authResult.Succeeded)
            {
                var deleted = await _service.DeleteReviewByIdAsync(reviewId);
                return Ok(deleted);
            }
            else
            {
                return Forbid();
            }

        }

        private Guid GetUserIdClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found");
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID format");
            }
            return userId;
        }
    }
}