using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Supertext.Base.Core.Configuration.Localization
{
    /// <summary>
    /// <para>Determines the culture information for a request via the value of a cookie.</para>
    /// <para>For Supertext, only the UI culture will be used as this is the culture which corresponds to resource files.</para>
    /// </summary>
    internal class CookieRequestCultureProvider : RequestCultureProvider
    {
        private static readonly char[] CookieSeparator =
            {
                '|'
            };
        private const string UiCulturePrefix = "uic=";

        /// <summary>
        /// Represent the default cookie name used to track the user's preferred culture information, which is ".AspNetCore.Culture".
        /// </summary>
        public static readonly string DefaultCookieName = ".AspNetCore.Culture";

        /// <summary>
        /// The name of the cookie that contains the user's preferred culture information.
        /// Defaults to <see cref="DefaultCookieName"/>.
        /// </summary>
        public string CookieName { get; } = DefaultCookieName;

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var cookie = httpContext.Request.Cookies[CookieName];

            if (String.IsNullOrEmpty(cookie))
            {
                return NullProviderCultureResult;
            }

            var providerResultCulture = ParseCookieValue(cookie);

            return Task.FromResult(providerResultCulture);
        }

        /// <summary>
        /// Parses a <see cref="RequestCulture"/> from the specified cookie value.
        /// Returns <c>null</c> if parsing fails.
        /// </summary>
        /// <param name="value">The cookie value to parse.</param>
        /// <returns>The <see cref="RequestCulture"/> or <c>null</c> if parsing fails.</returns>
        public static ProviderCultureResult ParseCookieValue(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var parts = value.Split(CookieSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return null;
            }

            var potentialUiCultureName = parts[1];

            if (!potentialUiCultureName.StartsWith(UiCulturePrefix))
            {
                return null;
            }

            var uiCultureName = potentialUiCultureName.Substring(UiCulturePrefix.Length);

            return new ProviderCultureResult(uiCultureName);
        }
    }
}