using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Supertext.Base.Core.Configuration.Specs")]

namespace Supertext.Base.Core.Configuration.Localization
{
    /// <summary>
    /// Determines the culture information for a request via the browser's specified list of preferred languages.
    /// </summary>
    internal class BrowserLanguagePreferenceProvider : RequestCultureProvider
    {
        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var languages = httpContext.Request
                                       .GetTypedHeaders()
                                       .AcceptLanguage
                                       ?.OrderByDescending(x => x.Quality ?? 1) // Quality defines priority from 0 to 1, where 1 is the highest.
                                       .Select(x => x.Value.ToString())
                                       .ToArray()
                            ?? Array.Empty<string>();

            var supertextCultureName = GetSupertextCultureName(DefaultCultures.SupertextDefaultCultures, languages)
                                       ?? GetSupertextLocaleAgnosticCultureName(DefaultCultures.SupertextDefaultCultures, languages)
                                       ?? DefaultCultures.DefaultCultureInfo.Name;

            var supertextUiCultureName = GetSupertextCultureName(DefaultCultures.SupertextDefaultUiCultures, languages)
                                         ?? GetSupertextLocaleAgnosticCultureName(DefaultCultures.SupertextDefaultUiCultures, languages)
                                         ?? DefaultCultures.DefaultUiCultureInfo.Name;

            return Task.FromResult(new ProviderCultureResult(supertextCultureName, supertextUiCultureName));
        }

        private static string GetSupertextCultureName(IList<CultureInfo> supertextCultures, IEnumerable<string> browserSpecifiedLanguages)
        {
            foreach (var browserSpecifiedLanguage in browserSpecifiedLanguages)
            {
                var supertextCulture = supertextCultures.FirstOrDefault(sci => String.Equals(sci.Name, browserSpecifiedLanguage, StringComparison.InvariantCultureIgnoreCase));

                if (supertextCulture != null)
                {
                    return supertextCulture.Name;
                }
            }

            return null;
        }

        private static string GetSupertextLocaleAgnosticCultureName(IList<CultureInfo> supertextCultures, IEnumerable<string> browserSpecifiedLanguages)
        {
            foreach (var browserSpecifiedLanguage in browserSpecifiedLanguages)
            {
                var supertextCulture = supertextCultures.FirstOrDefault(sci => String.Equals(sci.TwoLetterISOLanguageName, browserSpecifiedLanguage, StringComparison.InvariantCultureIgnoreCase));

                if (supertextCulture != null)
                {
                    return supertextCulture.Name;
                }
            }

            return null;
        }
    }
}