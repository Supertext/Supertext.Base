using System;
using System.Collections.Generic;
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

        public Option<IReadOnlyCollection<string>> GetRoles(ClaimsPrincipal claimsPrincipal)
        {
            const string roleClaimName = "role";

            var roleClaims = claimsPrincipal?.Claims?.Where(claim => claim.Type == roleClaimName)?.ToList();
            return roleClaims == null
                ? Option<IReadOnlyCollection<string>>.None()
                : Option<IReadOnlyCollection<string>>.Some(roleClaims.Select(roleClaim => roleClaim.Value).ToList());
        }

        public Option<T> GetValue<T>(ClaimsPrincipal claimsPrincipal, string claimName)
        {
            var nameClaim = claimsPrincipal?.Claims?.SingleOrDefault(claim => claim.Type == claimName);

            if (nameClaim != null)
            {
                object convertedValue;
                try
                {
                    convertedValue = Convert.ChangeType(nameClaim.Value, typeof(T));
                }
                catch (Exception)
                {
                    return Option<T>.None();
                }

                return Option<T>.Some((T)convertedValue);
            }

            return Option<T>.None();
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