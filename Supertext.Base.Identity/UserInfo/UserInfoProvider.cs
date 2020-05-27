using System;
using System.Linq;
using System.Security.Claims;
using Supertext.Base.Common;

namespace Supertext.Base.Identity.UserInfo
{
    internal class UserInfoProvider : IUserInfoProvider
    {
        public Option<long> GetSubjectId(ClaimsPrincipal claimsPrincipal)
        {
            var subClaim = claimsPrincipal?.Claims?.SingleOrDefault(claim => claim.Type == "sub");

            if (subClaim != null
                && Int64.TryParse(subClaim.Value, out var customerId))
            {
                return Option<long>.Some(customerId);
            }

            return Option<long>.None();
        }

        public Option<string> GetFirstName(ClaimsPrincipal claimsPrincipal)
        {
            return GetStringValue(claimsPrincipal, "given_name");
        }

        public Option<string> GetLastName(ClaimsPrincipal claimsPrincipal)
        {
            return GetStringValue(claimsPrincipal, "family_name");
        }

        private static Option<string> GetStringValue(ClaimsPrincipal claimsPrincipal, string claimName)
        {
            var nameClaim = claimsPrincipal?.Claims?.SingleOrDefault(claim => claim.Type == claimName);

            if (nameClaim != null)
            {
                return Option<string>.Some(nameClaim.Value);
            }

            return Option<string>.None();
        }
    }
}