using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UriBuilder = Supertext.Base.Net.Http.UriBuilder;

namespace Supertext.Base.Net.Specs.Http
{
    [TestClass]
    public class UriBuilderTests
    {
        private UriBuilder _testee;

        [TestInitialize]
        public void TestInitialize()
        {
            _testee = new UriBuilder();
        }

        [TestMethod]
        public void CreateAbsoluteUri_SimpleUrlIsGiven_Created()
        {
            const string relative = "api/v1/order/1234";
            _testee.AddDomain("www.supertext.ch");

            var result = _testee.CreateAbsoluteUri(relative);

            result.AbsoluteUri.Should().Be("https://www.supertext.ch/api/v1/order/1234");
        }

        [TestMethod]
        public void CreateAbsoluteUri_UrlWithSubdomainAndApiMethodIsGiven_Created()
        {
            const string relative = "api/v1/order/1234";
            _testee.AddDomain("www.cat.supertext.ch");

            var result = _testee.CreateAbsoluteUri(relative);

            result.AbsoluteUri.Should().Be("https://www.cat.supertext.ch/api/v1/order/1234");
        }

        [TestMethod]
        public void CreateAbsoluteUri_UrlWithQueryStringIsGiven_Created()
        {
            const string relative = "api/v1/order?orderId=123123";
            _testee.AddDomain("dev.supertext.ch");

            var result = _testee.CreateAbsoluteUri(relative);

            result.AbsoluteUri.Should().Be("https://dev.supertext.ch/api/v1/order?orderId=123123");
        }


        [TestMethod]
        public void ResolveUrl_SimpleUrlIsGiven_Created()
        {
            const string relative = "https://{domain}/api/v1/order/1234";
            _testee.AddDomain("www.supertext.ch");

            var result = _testee.ResolveUrl(relative);

            result.AbsoluteUri.Should().Be("https://www.supertext.ch/api/v1/order/1234");
        }

        [TestMethod]
        public void ResolveUrl_UrlWithSubdomainAndApiMethodIsGiven_Created()
        {
            const string relative = "https://www.cat.{domain}/api/v1/order/1234";
            _testee.AddDomain("supertext.ch");

            var result = _testee.ResolveUrl(relative);

            result.AbsoluteUri.Should().Be("https://www.cat.supertext.ch/api/v1/order/1234");
        }

        [TestMethod]
        public void ResolveUrl_UrlWithQueryStringIsGiven_Created()
        {
            const string relative = "https://{domain}/api/v1/order?orderId=123123";
            _testee.AddDomain("dev.supertext.ch");

            var result = _testee.ResolveUrl(relative);

            result.AbsoluteUri.Should().Be("https://dev.supertext.ch/api/v1/order?orderId=123123");
        }
    }
}