using System.Collections.Generic;

namespace Supertext.Base.Hosting.Middleware
{
    public interface ICorrelationIdExtractor
    {
        string Extract(IDictionary<object, object> contextItems);

        bool IsHandlingItem(IDictionary<object, object> contextItems);
    }
}