using System;
using System.Collections.Generic;
using log4net;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    internal class StrategyPolicyProvider : IRetryPolicyProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StrategyPolicyProvider));
        private readonly Lazy<RetryPolicy> _retryPolicyLazy;

        public StrategyPolicyProvider()
        {
            _retryPolicyLazy = new Lazy<RetryPolicy>(SetupRetryPolicy);
        }

        public RetryPolicy RetryPolicy
        {
            get { return _retryPolicyLazy.Value; }
        }

        private RetryPolicy SetupRetryPolicy()
        {
            Log.Info("Setting Database Retry Strategy.");
            const string defaultRetryStrategyName = "default";

            var strategy = new Incremental(defaultRetryStrategyName, 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            var strategies = new List<RetryStrategy> { strategy };
            var manager = new RetryManager(strategies, defaultRetryStrategyName);
            RetryManager.SetDefault(manager, false);
            var retryPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(strategy);
            retryPolicy.Retrying += (obj, eventArgs) => Log.Info($"Retrying, CurrentRetryCount = {eventArgs.CurrentRetryCount} , "
                                                                 + $"Delay = {eventArgs.Delay}, Exception = {eventArgs.LastException.Message}");
            return retryPolicy;
        }
    }
}