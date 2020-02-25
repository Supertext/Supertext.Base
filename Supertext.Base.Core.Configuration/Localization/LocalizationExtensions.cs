using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Core.Configuration.Localization
{
    public static class LocalizationExtensions
    {
        public static void AddSupertextLocalization(this IServiceCollection services, ICollection<CultureInfo> additionalCultures)
        {
            var cultures = new List<CultureInfo>(DefaultCultures.SupertextDefaultCultures);
            cultures.AddRange(additionalCultures);
            services.Configure<RequestLocalizationOptions>(options =>
                                                           {
                                                               options.DefaultRequestCulture = new RequestCulture(DefaultCultures.DefaultCultureInfo.Name);
                                                               options.SupportedCultures = cultures;
                                                               options.SupportedUICultures = cultures;
                                                               options.RequestCultureProviders = new List<IRequestCultureProvider>
                                                                                                 {
                                                                                                     new RouteDataLanguageProvider { RouteDataStringKey = "languageCode"},
                                                                                                     new CookieRequestCultureProvider()
                                                                                                 };
                                                           });
        }
    }
}