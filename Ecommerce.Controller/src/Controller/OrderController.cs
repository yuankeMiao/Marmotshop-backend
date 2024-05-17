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
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrdersAsync([FromQuery] OrderQueryOptions options)
        {
            var result = await _orderService.GetAllOrdersAsync(options);
            var orders = result.Data;
            var totalCount = result.TotalCount;

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderReadDto>> GetOrderByIdAsync([FromRoute] Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrdersByUserIdAsync([FromRoute] Guid userId, [FromQuery] OrderQueryOptions options)
        {
            var result = await _orderService.GetAllOrdersByUserIdAsync(userId, options);
            var orders = result.Data;
            var totalCount = result.TotalCount;

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetMyOrdersAsync([FromQuery] OrderQueryOptions options)
        {
            var userId = GetUserIdClaim();
            var orders = await _orderService.GetAllOrdersByUserIdAsync(userId, options);
            return Ok(orders);
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<OrderReadDto>> CreateOrderAsync([FromBody] OrderCreateDto orderCreateDto)
        {
            var userId = GetUserIdClaim();
            var order = await _orderService.CreateOrderWithtransactionAsync(userId, orderCreateDto);
            return Created($"http://localhost:5227/api/v1/orders/{order.Id}", order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderId}")]
        public async Task<ActionResult<OrderReadDto>> UpdateOrderByIdAsync([FromRoute] Guid orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            var order = await _orderService.UpdateOrderByIdAsync(orderId, orderUpdateDto);
            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<bool>> DeleteAnOrderByIdAsync([FromRoute] Guid orderId)
        {
            var deleted = await _orderService.DeleteOrderByIdAsync(orderId); // if deleteion failed, server will throw error
            return Ok(deleted);
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