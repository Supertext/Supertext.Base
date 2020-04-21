﻿using System.Security.Claims;
using Supertext.Base.Common;

namespace Supertext.Base.Identity.UserInfo
{
    public interface IUserInfoProvider
    {
        Option<long> GetSubjectId(ClaimsPrincipal claimsPrincipal);
    }
}