using System.Collections.Generic;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    public interface ISqlResultConverter
    {
        void InterpretUtcDates(IEnumerable<dynamic> results);
        void DecodeStructure(IEnumerable<dynamic> results);
    }
}