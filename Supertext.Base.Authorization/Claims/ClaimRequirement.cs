using Microsoft.AspNetCore.Authorization;
using Supertext.Base.Common;

namespace Supertext.Base.Authorization.Claims
{
    internal class ClaimRequirement : IAuthorizationRequirement
    {
        public ClaimRequirement(string claimType)
        {
            Validate.NotNullOrWhitespace(claimType, nameof(claimType));
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }
}