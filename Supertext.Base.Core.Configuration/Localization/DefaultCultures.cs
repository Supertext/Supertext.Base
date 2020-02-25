using System.Collections.Generic;
using System.Globalization;

namespace Supertext.Base.Core.Configuration.Localization
{
    public static class DefaultCultures
    {
        public static CultureInfo DefaultCultureInfo => CultureInfo.CreateSpecificCulture("de-CH");

        public static IList<CultureInfo> SupertextDefaultCultures =>
            new List<CultureInfo>
            {
                DefaultCultureInfo,
                CultureInfo.CreateSpecificCulture("de-DE"),
                CultureInfo.CreateSpecificCulture("en-US"),
                CultureInfo.CreateSpecificCulture("fr-CH"),
                CultureInfo.CreateSpecificCulture("fr-FR"),
                CultureInfo.CreateSpecificCulture("it-CH"),
                CultureInfo.CreateSpecificCulture("it-IT"),
            };
    }
}