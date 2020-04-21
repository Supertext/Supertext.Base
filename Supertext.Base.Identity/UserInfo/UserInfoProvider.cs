using System;
using System.Linq;
using System.Security.Claims;
using Supertext.Base.Common;

namespace Supertext.Base.Identity.UserInfo
{
    internal class UserInfoProvider
    {
        public Option<long> ObtainSubjectId(ClaimsPrincipal claimsPrincipal)
        {
            var subClaim = claimsPrincipal?.Claims?.SingleOrDefault(claim => claim.Type == "sub");

            if (subClaim != null
                && Int64.TryParse(subClaim.Value, out var customerId))
            {
                return Option<long>.Some(customerId);
            }

            return Option<long>.None();
        }
    }
}