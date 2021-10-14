using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Supertext.Base.Resolver.Url
{
    public static class ConfigurationExtension
    {
        private const string DomainPlaceholder = "{domain}";

        public static void ReplacePlaceholders(this IConfiguration configuration, string domain)
        {
            foreach (var (key, value) in configuration.AsEnumerable().Where(kv => !String.IsNullOrWhiteSpace(kv.Value) && kv.Value.Contains(DomainPlaceholder, StringComparison.OrdinalIgnoreCase)))
            {
                var replacedValue = value.Replace(DomainPlaceholder, domain, StringComparison.OrdinalIgnoreCase);

                configuration[key] = replacedValue;
            }
        }
    }
}