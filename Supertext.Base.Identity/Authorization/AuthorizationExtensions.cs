using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Identity.Authorization
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Extension method for API authorization configuration.
        /// Make sure, services.AddAuthorization() is being added.
        /// It registers components as AuthorizationPolicyProvider and ClaimAuthorizeHandler.
        /// Those components are helpful for API authorization with decorated controllers by the AuthorizeByClaimAttribute.
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterClaimAuthorizationComponents(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, ClaimAuthorizeHandler>();
        }
    }
}