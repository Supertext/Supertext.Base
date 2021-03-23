using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UriBuilder = Supertext.Base.Net.Http.UriBuilder;

namespace Supertext.Base.Net.Specs.Http
{
    [TestClass]
    public class UriBuilderTests
    {
        private UriBuilder _testee;
        private IHttpContextAccessor _contextAccessor;

        [TestInitialize]
        public void TestInitialize()
        {
            _contextAccessor = A.Fake<IHttpContextAccessor>();
            _testee = new UriBuilder(_contextAccessor);
        }

        [TestMethod]
        public void CreateAbsoluteUri_SimpleUrlIsGiven_Created()
        {
            const string relative = "api/v1/order/1234";
            SetupHttpContext("https://www.supertext.ch");

            var result = _testee.CreateAbsoluteUri(relative);

            result.AbsoluteUri.Should().Be("https://www.supertext.ch/api/v1/order/1234");
        }

        [TestMethod]
        public void CreateAbsoluteUri_UrlWithSubdomainAndApiMethodIsGiven_Created()
        {
            const string relative = "api/v1/order/1234";
            SetupHttpContext("https://www.cat.supertext.ch/api/v1/order/1234");

            var result = _testee.CreateAbsoluteUri(relative);

            result.AbsoluteUri.Should().Be("https://www.cat.supertext.ch/api/v1/order/1234");
        }

        [TestMethod]
        public void CreateAbsoluteUri_UrlWithQueryStringIsGiven_Created()
        {
            const string relative = "/api/v1/order?orderId=123123";
            SetupHttpContext("https://dev.supertext.ch/api/v1/order?orderId=123123");

            var result = _testee.CreateAbsoluteUri(relative);

            result.AbsoluteUri.Should().Be("https://dev.supertext.ch/api/v1/order?orderId=123123");
        }

        [TestMethod]
        public void BaseUri_SimpleUrlIsGiven_BaseUriIsReturned()
        {
            SetupHttpContext("https://www.supertext.ch");

            var result = _testee.BaseUri;

            result.AbsoluteUri.Should().Be("https://www.supertext.ch/");
        }

        [TestMethod]
        public void BaseUri_SimpleUrlAndApiMethodIsGiven_BaseUriIsReturned()
        {
            SetupHttpContext("https://www.supertext.ch/api/v1/order/1234");

            var result = _testee.BaseUri;

            result.AbsoluteUri.Should().Be("https://www.supertext.ch/");
        }

        [TestMethod]
        public void BaseUri_UrlWithSubdomainIsGiven_BaseUriIsReturned()
        {
            SetupHttpContext("https://www.cat.supertext.ch");

            var result = _testee.BaseUri;

            result.AbsoluteUri.Should().Be("https://www.cat.supertext.ch/");
        }

        [TestMethod]
        public void BaseUri_UrlWithSubdomainAndApiMethodIsGiven_BaseUriIsReturned()
        {
            SetupHttpContext("https://www.cat.supertext.ch/api/v1/order/1234");

            var result = _testee.BaseUri;

            result.AbsoluteUri.Should().Be("https://www.cat.supertext.ch/");
        }

        [TestMethod]
        public void BaseUri_UrlWithSubdomain2AndApiMethodIsGiven_BaseUriIsReturned()
        {
            SetupHttpContext("https://dev.supertext.ch/api/v1/order/1234");

            var result = _testee.BaseUri;

            result.AbsoluteUri.Should().Be("https://dev.supertext.ch/");
        }

        [TestMethod]
        public void BaseUri_UrlWithQueryStringIsGiven_BaseUriIsReturned()
        {
            SetupHttpContext("https://dev.supertext.ch/api/v1/order?orderId=123123");

            var result = _testee.BaseUri;

            result.AbsoluteUri.Should().Be("https://dev.supertext.ch/");
        }

        private void SetupHttpContext(string url)
        {
            UriHelper.FromAbsolute(url,
                                   out var scheme,
                                   out var host,
                                   out var path,
                                   out var query,
                                   fragment: out var _);

            var context = new DefaultHttpContext();
            context.Request.Scheme = scheme;
            context.Request.Host = host;
            context.Request.Path = path;
            context.Request.QueryString = query;

            A.CallTo(() => _contextAccessor.HttpContext).Returns(context);
        }
    }
}