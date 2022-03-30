using System.Collections.Generic;
using System.Globalization;

namespace Supertext.Base.Core.Configuration.Localization
{
    public static class DefaultCultures
    {
        public static CultureInfo DefaultCultureInfo => CultureInfo.GetCultureInfo("de-CH");

        public static CultureInfo DefaultUICultureInfo => DefaultCultureInfo;

        public static IList<CultureInfo> SupertextDefaultCultures =>
            new List<CultureInfo>
            {
                DefaultCultureInfo,
                CultureInfo.GetCultureInfo("de-DE"),
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("en-GB"),
                CultureInfo.GetCultureInfo("fr-CH"),
                CultureInfo.GetCultureInfo("fr-FR"),
                CultureInfo.GetCultureInfo("it-CH"),
                CultureInfo.GetCultureInfo("it-IT")
            };

        public static IList<CultureInfo> SupertextDefaultUICultures =>
            new List<CultureInfo>
            {
                DefaultCultureInfo,
                CultureInfo.GetCultureInfo("de-DE"),
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("fr-CH"),
                CultureInfo.GetCultureInfo("fr-FR"),
                CultureInfo.GetCultureInfo("it-CH"),
                CultureInfo.GetCultureInfo("it-IT")
            };
    }
}