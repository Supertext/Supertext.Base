using Microsoft.AspNetCore.Authorization;

namespace Supertext.Base.Authorization.Claims
{
    public class AuthorizeByClaimAttribute : AuthorizeAttribute
    {
        public AuthorizeByClaimAttribute(string claimValue) => ClaimValue = claimValue;

        // Get or set the ClaimValue property by manipulating the underlying Policy property
        public string ClaimValue
        {
            get
            {
                return Policy;
            }
            set
            {
                Policy = value;
            }
        }
    }
}
