using System;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Supertext.Base.SqlServer.Utils
{
    internal class DefaultRetryPolicyProvider : IRetryPolicyProvider
    {
        private readonly Lazy<RetryPolicy> _retryPolicyLazy;

        public DefaultRetryPolicyProvider()
        {
            _retryPolicyLazy  = new Lazy<RetryPolicy>(() => RetryPolicy.DefaultProgressive);
        }

        public RetryPolicy RetryPolicy
        {
            get { return _retryPolicyLazy.Value; }
        }
    }
}