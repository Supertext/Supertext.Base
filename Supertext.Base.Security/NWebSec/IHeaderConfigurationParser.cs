using System.Collections.Generic;

namespace Supertext.Base.Security.NWebSec
{
    internal interface IHeaderConfigurationParser
    {
        IEnumerable<string> Parse(string source);
    }
}