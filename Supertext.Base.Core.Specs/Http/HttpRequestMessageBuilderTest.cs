using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Authentication;
using Supertext.Base.Factory;
using Supertext.Base.Net.Http;
using Supertext.Base.Tracing;

namespace Supertext.Base.Net.Specs.Http
{
    [TestClass]
    public class HttpRequestMessageBuilderTest
    {
        private const string AuthorizationHeader = "Authorization";
        private const string CorrelationId = "My correlation id";
        private const string CorrelationIdHeaderName = "headerName";
        private const string Token = "My token";
        private HttpRequestMessageBuilder _testee;
        private ITokenProvider _tokenProvider;
        private ITracingProvider _tracingProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _tokenProvider = A.Fake<ITokenProvider>();
            _tracingProvider = A.Fake<ITracingProvider>();
            var logger = A.Fake<ILogger<HttpRequestMessageBuilder>>();
            var factory = A.Fake<IFactory>();
            _testee = new HttpRequestMessageBuilder(factory, logger);

            A.CallTo(() => _tokenProvider.RetrieveAccessTokenAsync(A<string>._,
                                                                   A<string>._,
                                                                   A<AlternativeAuthorityDetails>._,
                                                                   null))
             .Returns(Token);

            A.CallTo(() => _tracingProvider.CorrelationIdHeaderName).Returns(CorrelationIdHeaderName);
            A.CallTo(() => _tracingProvider.CorrelationIdDigitsFormat).Returns(CorrelationId);
            A.CallTo(() => factory.Create<ITokenProvider>()).Returns(_tokenProvider);
            A.CallTo(() => factory.Create<ITracingProvider>()).Returns(_tracingProvider);
        }

        [TestMethod]
        public async Task BuildAsync_UseOfAllStepsAreIncluded_HttpRequestMessageIsBuiltAccordingly()
        {
            const string requestUri = "/api/bla";
            const string clientId = "Some client";
            const string sub = "4711";
            var builder = _testee.Create(HttpMethod.Get, requestUri)
                                 .UseBearerToken(clientId, sub)
                                 .UseCorrelationId();

            var result = await builder.BuildAsync();

            result.Should().NotBeNull();
            result.Headers.GetValues(AuthorizationHeader).Single().Should().Be($"Bearer {Token}");
            result.Headers.GetValues(CorrelationIdHeaderName).Single().Should().Be(CorrelationId);
        }

        [TestMethod]
        public async Task BuildAsync_UseOfAllStepsAreIncludedButDifferentInvocationOrder_HttpRequestMessageIsBuiltAccordingly()
        {
            const string requestUri = "/api/bla";
            const string clientId = "Some client";
            const string sub = "4711";
            var builder = _testee.UseBearerToken(clientId, sub)
                                 .UseCorrelationId()
                                 .Create(HttpMethod.Get, requestUri);

            var result = await builder.BuildAsync();

            result.Should().NotBeNull();
            result.Headers.GetValues(AuthorizationHeader).Single().Should().Be($"Bearer {Token}");
            result.Headers.GetValues(CorrelationIdHeaderName).Single().Should().Be(CorrelationId);
        }

        [TestMethod]
        public async Task BuildAsync_NoCorrelationIdAdded_HttpRequestMessageIsBuiltWithoutCorrelationId()
        {
            const string requestUri = "/api/bla";
            const string clientId = "Some client";
            const string sub = "4711";
            var builder = _testee.Create(HttpMethod.Get, requestUri)
                                 .UseBearerToken(clientId, sub);

            var result = await builder.BuildAsync();

            result.Should().NotBeNull();
            result.Headers.GetValues(AuthorizationHeader).Single().Should().Be($"Bearer {Token}");
            result.Headers.Contains(CorrelationIdHeaderName).Should().BeFalse();
        }

        [TestMethod]
        public async Task BuildAsync_NoTokenAdded_HttpRequestMessageIsBuiltWithoutToken()
        {
            const string requestUri = "/api/bla";
            var builder = _testee.Create(HttpMethod.Get, requestUri)
                                 .UseCorrelationId();

            var result = await builder.BuildAsync();

            result.Should().NotBeNull();
            result.Headers.Contains(AuthorizationHeader).Should().BeFalse();
            result.Headers.GetValues(CorrelationIdHeaderName).Single().Should().Be(CorrelationId);
        }

        [TestMethod]
        public async Task BuildAsync_OnlyPlain_HttpRequestMessageIsBuiltWithoutTokenNorCorrelationId()
        {
            const string requestUri = "/api/bla";
            var builder = _testee.Create(HttpMethod.Get, requestUri);

            var result = await builder.BuildAsync();

            result.Should().NotBeNull();
            result.Headers.Contains(AuthorizationHeader).Should().BeFalse();
            result.Headers.Contains(CorrelationIdHeaderName).Should().BeFalse();
        }

        [TestMethod]
        public async Task BuildAsync_WithTwoBuildersRunningConcurrently_TwoHttpRequestMessages()
        {
            const string requestUri = "/api/bla";
            const string clientId = "Some client";
            const string sub = "4711";
            var builder1 = _testee.UseBearerToken(clientId, sub)
                                  .UseCorrelationId()
                                  .Create(HttpMethod.Get, requestUri);
            var builder2 = _testee.UseBearerToken(clientId, sub)
                                  .UseCorrelationId()
                                  .Create(HttpMethod.Get, requestUri);

            var result1 = await builder1.BuildAsync();
            var result2 = await builder2.BuildAsync();

            result1.Should().NotBeNull();
            result1.Should().NotBe(result2);
            result1.Headers.GetValues(AuthorizationHeader).Single().Should().Be($"Bearer {Token}");
            result1.Headers.GetValues(CorrelationIdHeaderName).Single().Should().Be(CorrelationId);
        }
    }
}