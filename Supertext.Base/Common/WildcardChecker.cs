using System.Text.RegularExpressions;

namespace Supertext.Base.Common
{
    internal class WildcardChecker : IWildcardChecker
    {
        public bool IsPassing(string filter, string value)
        {
            var regex = ConvertToRegex(filter);

            return regex.IsMatch(value);
        }

        private static Regex ConvertToRegex(string filter)
        {
            var wildcardReplacedFilter = filter.Replace("*", ".*");

            return new Regex($"^{wildcardReplacedFilter}$");
        }
    }
}