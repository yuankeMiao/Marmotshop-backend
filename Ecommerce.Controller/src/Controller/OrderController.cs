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
        public async Task<ActionResult<QueryResult<OrderReadDto>>> GetAllOrdersAsync([FromQuery] OrderQueryOptions options)
        {
            var result = await _orderService.GetAllOrdersAsync(options);

            return Ok(result);
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
        public async Task<ActionResult<QueryResult<OrderReadDto>>> GetAllOrdersByUserIdAsync([FromRoute] Guid userId, [FromQuery] OrderQueryOptions options)
        {
            var result = await _orderService.GetAllOrdersByUserIdAsync(userId, options);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-orders")]
        public async Task<ActionResult<QueryResult<OrderReadDto>>> GetMyOrdersAsync([FromQuery] OrderQueryOptions options)
        {
            var userId = GetUserIdClaim();
            var result = await _orderService.GetAllOrdersByUserIdAsync(userId, options);
            return Ok(result);
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<OrderReadDto>> CreateOrderAsync([FromBody] OrderCreateDto orderCreateDto)
        {
            var userId = GetUserIdClaim();
            var order = await _orderService.CreateOrderWithTransactionAsync(userId, orderCreateDto);
            return StatusCode(201, order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}")]
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