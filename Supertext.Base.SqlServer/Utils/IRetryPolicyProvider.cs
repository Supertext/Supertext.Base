using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Supertext.Base.SqlServer.Utils
{
    internal interface IRetryPolicyProvider
    {
        RetryPolicy RetryPolicy { get; }
    }
}