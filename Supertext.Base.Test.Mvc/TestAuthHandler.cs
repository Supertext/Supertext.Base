using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Supertext.Base.Test.Mvc
{
    internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string AuthenticationScheme = "Test";
        private readonly TestSettings _testSettings;

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                               ILoggerFactory logger,
                               UrlEncoder encoder,
                               TestSettings testSettings,
#pragma warning disable CS0618 // Type or member is obsolete
                               ISystemClock clock)
            : base(options, logger, encoder, clock)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            _testSettings = testSettings;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authHeader = Request.Headers["Authorization"];
            var identity = new ClaimsIdentity(new List<Claim>(), AuthenticationScheme);

            if (authHeader?.StartsWith(AuthenticationScheme) == true)
            {
                var userId = Convert.ToInt64(authHeader.Substring(AuthenticationScheme.Length + 1));
                identity = new ClaimsIdentity(_testSettings?.UserClaims[userId], AuthenticationScheme);
            }

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}