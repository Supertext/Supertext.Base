using System.Data;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.SqlServer.Specs
{
    [TestClass]
    public class UnitOfWorkTest
    {
        private ISqlConnectionFactory _sqlConnectionFactory;
        private IUnitOfWork _testee;
        private IDbConnection _dbConnection;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbConnection = A.Fake<IDbConnection>();
            _sqlConnectionFactory = A.Fake<ISqlConnectionFactory>();
            _testee = new UnitOfWork("someConnectionString",
                                     _sqlConnectionFactory);

            A.CallTo(() => _sqlConnectionFactory.CreateOpenedReliableConnection(A<string>.Ignored)).Returns(_dbConnection);
        }

        [TestMethod]
        public void ExecuteWithinTransactionScope_WhenDBConnectionIsProvided_TransactionIsExecuted()
        {
            // Act
            _testee.ExecuteWithinTransactionScope(connection => { connection.Should().Be(_dbConnection); });

            // Assert
            A.CallTo(() => _sqlConnectionFactory.CreateOpenedReliableConnection(A<string>.Ignored)).MustHaveHappened();
        }

        [TestMethod]
        public async Task ExecuteWithinTransactionScopeAsync_WhenDBConnectionIsProvided_TransactionIsExecuted()
        {
            // Act
            await _testee.ExecuteWithinTransactionScopeAsync(connection =>
                                                             {
                                                                 connection.Should().Be(_dbConnection);
                                                                 return Task.CompletedTask;
                                                             });

            // Assert
            A.CallTo(() => _sqlConnectionFactory.CreateOpenedReliableConnection(A<string>.Ignored)).MustHaveHappened();
        }

        [TestMethod]
        public void ExecuteScalar_QueryExecuted_ExpectedValueReturned()
        {
            const string someValue = "a value";

            var result = _testee.ExecuteScalar(connection => someValue);

            result.Should().Be(someValue);
        }

        [TestMethod]
        public async Task ExecuteScalar_QueryExecutedAsync_ExptectedValueReturned()
        {
            const string someValue = "a value";

            var result = await _testee.ExecuteScalarAsync(connection => Task.FromResult(someValue));

            result.Should().Be(someValue);
        }
    }
}