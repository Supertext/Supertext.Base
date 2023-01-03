using System.Collections.Generic;
using System.Security.Claims;

namespace Supertext.Base.Test.Mvc
{
    internal class TestSettings
    {
        private readonly Dictionary<long, List<Claim>> _userClaims;

        public TestSettings()
        {
            _userClaims = new Dictionary<long, List<Claim>>();
        }

        public IReadOnlyDictionary<long, List<Claim>> UserClaims => _userClaims;

        public void AddClaim(long userId, Claim claim)
        {
            if (!_userClaims.ContainsKey(userId))
            {
                _userClaims.Add(userId, new List<Claim>());
            }

            _userClaims[userId].Add(claim);
        }
    }
}