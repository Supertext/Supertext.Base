using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Supertext.Base.Authorization.Claims
{
    internal class ClaimAuthorizeHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "allowedAccess" && c.Value == requirement.ClaimValue))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}