#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Core.Configuration.Localization
{
    public static class LocalizationExtensions
    {
        public static void AddSupertextLocalization(this IServiceCollection services, ICollection<CultureInfo> additionalCultures = null)
        {
            var cultures = new List<CultureInfo>(DefaultCultures.SupertextDefaultCultures);
            if (additionalCultures != null)
            {
                cultures.AddRange(additionalCultures);
            }

            var uiCultures = new List<CultureInfo>(DefaultCultures.SupertextDefaultUiCultures);
            if (additionalCultures != null)
            {
                uiCultures.AddRange(additionalCultures);
            }

            services.Configure<RequestLocalizationOptions>(options =>
                                                           {
                                                               options.DefaultRequestCulture = new RequestCulture(DefaultCultures.DefaultCultureInfo.Name);
                                                               options.SupportedCultures = cultures;
                                                               options.SupportedUICultures = uiCultures;
                                                               options.RequestCultureProviders = new List<IRequestCultureProvider>
                                                                                                 {
                                                                                                     new RouteDataLanguageProvider { RouteDataStringKey = "languageCode"},
                                                                                                     new CookieRequestCultureProvider(),
                                                                                                     new BrowserLanguagePreferenceProvider()
                                                                                                 };
                                                           });
        }
    }
}
#endif