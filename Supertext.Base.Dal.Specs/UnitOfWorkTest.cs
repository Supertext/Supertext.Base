using System.Data;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.Specs
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
    }
}