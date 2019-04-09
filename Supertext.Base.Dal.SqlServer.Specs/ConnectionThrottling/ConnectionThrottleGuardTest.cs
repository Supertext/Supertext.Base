using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.ConnectionThrottling;

namespace Supertext.Base.Dal.SqlServer.Specs.ConnectionThrottling
{
    [TestClass]
    [Ignore("The ConnectionThrottleGuard is hard to be unit testable. "
            + "Containing test methods show sync and async invocation of the guard.")]
    public class ConnectionThrottleGuardTest
    {
        private IConnectionThrottleGuard _testee;

        [TestInitialize]
        public void TestInitialize()
        {
            _testee = new ConnectionThrottleGuard(new ThrottlingConfig{MaxCountOfConcurrentSqlConnections = 2});
        }

        [TestMethod]
        public void Create_ConnectionStringIsValid_UnitOfWorkIsCreated()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => Execute()));
            }

            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public async Task CreateAsync_ConnectionStringIsValid_UnitOfWorkIsCreated()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(async () => await ExecuteAsync()));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private void Execute()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} about to start");
            using (_testee.ExecuteGuarded())
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} started");
                Thread.Sleep(500);
            }
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished");
        }

        private async Task ExecuteAsync()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} about to start");
            using (await _testee.ExecuteGuardedAsync())
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} started");
                await Task.Delay(500);
            }
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished");
        }
    }
}