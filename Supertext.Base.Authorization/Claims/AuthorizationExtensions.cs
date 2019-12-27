using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Authorization.Claims
{
    public static class AuthorizationExtensions
    {
        public static void RegisterClaimAuthorizationComponents(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, ClaimAuthorizeHandler>();
        }
    }
}