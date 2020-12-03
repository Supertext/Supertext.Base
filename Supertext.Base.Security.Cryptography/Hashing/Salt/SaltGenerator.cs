using System.Globalization;
using NodaTime;

namespace Supertext.Base.Security.Cryptography.Hashing.Salt
{
    internal class SaltGenerator : ISaltGenerator
    {
        public string Generate()
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var timeStampForSalt = now.InUtc().ToString("yyyy-MM-dd HH:mm:ss.fffffff",
                                                        CultureInfo.InvariantCulture);
            return timeStampForSalt;
        }
    }
}