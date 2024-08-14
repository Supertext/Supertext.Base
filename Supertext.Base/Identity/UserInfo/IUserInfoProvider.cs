using Supertext.Base.Common;
using System.Collections.Generic;
using System.Security.Claims;

namespace Supertext.Base.Identity.UserInfo
{
    public interface IUserInfoProvider
    {
        Option<long> GetSubjectId(ClaimsPrincipal claimsPrincipal);

        Option<string> GetFirstName(ClaimsPrincipal claimsPrincipal);

        Option<string> GetLastName(ClaimsPrincipal claimsPrincipal);

        IReadOnlyCollection<string> GetRoles(ClaimsPrincipal claimsPrincipal);

        Option<T> GetValue<T>(ClaimsPrincipal claimsPrincipal, string claimName);
    }
}