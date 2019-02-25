using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    internal interface IRetryPolicyProvider
    {
        RetryPolicy RetryPolicy { get; }
    }
}