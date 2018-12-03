using System;

namespace Supertext.Base.Common
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
        DateTime UtcToday { get; }
    }
}