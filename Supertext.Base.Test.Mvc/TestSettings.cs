using System.Collections.Generic;
using System.Security.Claims;

namespace Supertext.Base.Test.Mvc
{
    internal class TestSettings
    {
        private readonly List<Claim> _userClaims;

        public TestSettings()
        {
            _userClaims = new List<Claim>();
        }

        public IEnumerable<Claim> UserClaims => _userClaims;

        public void AddClaim(Claim claim)
        {
            _userClaims.Add(claim);
        }
    }
}