using System.Collections.Generic;

namespace Supertext.Base.Dal
{
    public interface ISqlResultConverter
    {
        TEntity InterpretUtcDates<TEntity>(TEntity entity) where TEntity : class;
        IDictionary<string, object> DecodeStructure(IDictionary<string, object> row);
    }
}