using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Core.Configuration.Localization
{
    public static class LocalizationExtensions
    {
        public static void AddLocalization(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
                                                           {
                                                               options.DefaultRequestCulture = new RequestCulture(DefaultCultures.DefaultCultureInfo.Name);
                                                               options.SupportedCultures = DefaultCultures.SupertextDefaultCultures;
                                                               options.SupportedUICultures = DefaultCultures.SupertextDefaultCultures;
                                                               options.RequestCultureProviders = new List<IRequestCultureProvider>
                                                                                                 {
                                                                                                     new RouteDataLanguageProvider { RouteDataStringKey = "languageCode"},
                                                                                                     new CookieRequestCultureProvider()
                                                                                                 };
                                                           });
        }
    }
}