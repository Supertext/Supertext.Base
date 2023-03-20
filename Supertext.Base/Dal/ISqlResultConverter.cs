using System.Collections.Generic;

namespace Supertext.Base.Dal
{
    public interface ISqlResultConverter
    {
        IDictionary<string, object> InterpretUtcDates(IDictionary<string, object> row);
        IDictionary<string, object> DecodeStructure(IDictionary<string, object> row);
    }
}