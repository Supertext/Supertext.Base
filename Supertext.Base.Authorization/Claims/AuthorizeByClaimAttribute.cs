using Microsoft.AspNetCore.Authorization;

namespace Supertext.Base.Authorization.Claims
{
    public class AuthorizeByClaimAttribute : AuthorizeAttribute
    {
        public AuthorizeByClaimAttribute(string claimType) => ClaimType = claimType;

        // Get or set the ClaimType property by manipulating the underlying Policy property
        public string ClaimType
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
