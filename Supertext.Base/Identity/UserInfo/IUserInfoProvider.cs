using System.Security.Claims;
using Supertext.Base.Common;

namespace Supertext.Base.Identity.UserInfo
{
    public interface IUserInfoProvider
    {
        Option<long> GetSubjectId(ClaimsPrincipal claimsPrincipal);

        Option<string> GetFirstName(ClaimsPrincipal claimsPrincipal);

        Option<string> GetLastName(ClaimsPrincipal claimsPrincipal);

        Option<T> GetValue<T>(ClaimsPrincipal claimsPrincipal, string claimName);
    }
}