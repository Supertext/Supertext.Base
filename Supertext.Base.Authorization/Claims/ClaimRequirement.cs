using Microsoft.AspNetCore.Authorization;
using Supertext.Base.Common;

namespace Supertext.Base.Authorization.Claims
{
    internal class ClaimRequirement : IAuthorizationRequirement
    {
        public ClaimRequirement(string claimValue)
        {
            Validate.NotNullOrWhitespace(claimValue, nameof(claimValue));
            ClaimValue = claimValue;
        }

        public string ClaimValue { get; }
    }
}