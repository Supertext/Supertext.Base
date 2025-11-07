using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using Supertext.Base.Common;
using Supertext.Base.Net.Http;

namespace Supertext.Base.Net.Specs.Http
{
    [TestClass]
    public class TokenCacheTest
    {
        private const string TestClientId = "clientId";
        private const string TestDelegationSub = "delegationSub";
        private const string TestHttpClientName = "httpClientName";

        private TokenCache _testee;
        private IDateTimeProvider _dateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _dateTimeProvider = A.Fake<IDateTimeProvider>();

            _testee = new TokenCache(_dateTimeProvider, A.Fake<ILogger<TokenCache>>());
        }

        [TestMethod]
        public void GetToken_TokenIsNotInCache_ReturnsNone()
        {
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());
            Assert.IsTrue(result.IsNone);
        }

        [TestMethod]
        public void GetToken_TokenIsExpired_ReturnsNone()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };


            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);

            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());


            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow.AddHours(1));

            // Act
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());

            // Assert
            Assert.IsTrue(result.IsNone);
        }

        [TestMethod]
        public void GetToken_TokenIsValid_ReturnsToken()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);

            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());

            // Act
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());
            // Assert
            Assert.IsTrue(result.IsSome);
            Assert.AreEqual(token.AccessToken, result.Value.AccessToken);
        }

        [TestMethod]
        public void GetToken_TokenForDifferentClientId_ReturnsNone()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);

            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());

            // Act
            var result = _testee.GetToken("differentClientId",
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());

            // Assert
            Assert.IsTrue(result.IsNone);
        }

        [TestMethod]
        public void GetToken_TokenForDifferentDelegationSub_ReturnsNone()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);

            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());

            // Act
            var result = _testee.GetToken(TestClientId,
                                          "differentDelegationSub",
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());

            // Assert
            Assert.IsTrue(result.IsNone);
        }

        [TestMethod]
        public void GetToken_TokenForDifferentHttpClientName_ReturnsNone()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());
            // Act
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          "differentHttpClientName",
                                          null,
                                          new Dictionary<string, string>());
            // Assert
            Assert.IsTrue(result.IsNone);
        }

        [TestMethod]
        public void GetToken_TokenWithDifferentClaims_ReturnsNone()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());

            // Act
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>
                                          {
                                              { "claim1", "value1" }
                                          });

            // Assert
            Assert.IsTrue(result.IsNone);
        }

        [TestMethod]
        public void GetToken_TokenWithSameClaimsDifferentOrder_ReturnsToken()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "aoseifjapo",
                            ExpiresIn = 3600
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>
                                     {
                                         { "claim1", "value1" },
                                         { "claim2", "value2" }
                                     });
            // Act
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>
                                          {
                                              { "claim2", "value2" },
                                              { "claim1", "value1" }
                                          });
            // Assert
            Assert.IsTrue(result.IsSome);
            Assert.AreEqual(token.AccessToken, result.Value.AccessToken);
        }

        [TestMethod]
        public void AddOrUpdateToken_TokenIsUpdated_CacheIsUpdated()
        {
            // Arrange
            var token1 = new TokenResponseDto
                         {
                             AccessToken = "token1",
                             ExpiresIn = 3600
                         };
            var token2 = new TokenResponseDto
                         {
                             AccessToken = "token2",
                             ExpiresIn = 3600
                         };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
            _testee.AddOrUpdateToken(token1,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());
            // Act
            _testee.AddOrUpdateToken(token2,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());
            // Assert
            Assert.IsTrue(result.IsSome);
            Assert.AreEqual(token2.AccessToken, result.Value.AccessToken);
        }

        [TestMethod]
        public void AddOrUpdateToken_TokenWithNoAccessTokenAndShortExpiry_IsNotCached()
        {
            // Arrange
            var token = new TokenResponseDto
                        {
                            AccessToken = "",
                            ExpiresIn = 100 // Less than MinValidityForCachingInSeconds
                        };
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
            // Act
            _testee.AddOrUpdateToken(token,
                                     TestClientId,
                                     TestDelegationSub,
                                     TestHttpClientName,
                                     null,
                                     new Dictionary<string, string>());
            var result = _testee.GetToken(TestClientId,
                                          TestDelegationSub,
                                          TestHttpClientName,
                                          null,
                                          new Dictionary<string, string>());
            // Assert
            Assert.IsTrue(result.IsNone);
        }
    }
}