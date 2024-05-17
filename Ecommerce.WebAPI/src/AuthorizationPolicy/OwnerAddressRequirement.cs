using System.Security.Claims;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.WebAPI.src.AuthorizationPolicy
{
    public class OwnerAddressRequirement : IAuthorizationRequirement
    {
        public OwnerAddressRequirement()
        {
        }
    }

    public class OwnerAddressHandler : AuthorizationHandler<OwnerAddressRequirement, AddressReadDto>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerAddressRequirement requirement, AddressReadDto addressReadDto)
        {
            var claims = context.User.Claims;
            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == addressReadDto.UserId.ToString())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}