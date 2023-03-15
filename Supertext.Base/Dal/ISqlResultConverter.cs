using System.Collections.Generic;

namespace Supertext.Base.Dal
{
    public interface ISqlResultConverter
    {
        void InterpretUtcDates(IEnumerable<IDictionary<string, object>> results);
        void DecodeStructure(IEnumerable<IDictionary<string, object>> results);
    }
}