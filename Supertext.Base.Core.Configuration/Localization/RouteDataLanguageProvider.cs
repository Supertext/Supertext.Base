using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;

namespace Supertext.Base.Core.Configuration.Localization
{
    /// <summary>
    /// Determines the culture information for a request via values in the route data.
    /// </summary>
    internal class RouteDataLanguageProvider : RequestCultureProvider
    {
        /// <summary>
        /// The key that contains the culture name.
        /// Defaults to "languageCode".
        /// </summary>
        public string RouteDataStringKey { get; set; } = "languageCode";

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string culture = null;

            if (!String.IsNullOrEmpty(RouteDataStringKey))
            {
                culture = httpContext.GetRouteValue(RouteDataStringKey)?.ToString();
            }

            if (culture == null)
            {
                // No values specified for either so no match
                return NullProviderCultureResult;
            }

            var supportedCulture = DefaultCultures.SupertextDefaultCultures.FirstOrDefault(ci => culture.Equals(ci.TwoLetterISOLanguageName,
                                                                                                                StringComparison.InvariantCultureIgnoreCase));
            return supportedCulture != null
                       ? Task.FromResult(new ProviderCultureResult(supportedCulture.Name))
                       : NullProviderCultureResult;
        }
    }
}