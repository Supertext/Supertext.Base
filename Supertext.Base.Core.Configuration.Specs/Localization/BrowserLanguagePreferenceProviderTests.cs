using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Core.Configuration.Localization;
using System.Linq;

namespace Supertext.Base.Core.Configuration.Specs.Localization
{
    [TestClass]
    public class BrowserLanguagePreferenceProviderTests
    {
        private BrowserLanguagePreferenceProvider _testee;

        [TestInitialize]
        public void TestInit()
        {
            _testee = new BrowserLanguagePreferenceProvider();
        }

        [TestMethod]
        public void DetermineProviderCultureResult_WhenContainsRecognisedCultures_ThenReturnsFirstRecognisedCultures()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.AcceptLanguage] = "fr-CH, fr;q=0.9, en;q=0.8, de;q=0.7, *;q=0.5";

            var result = _testee.DetermineProviderCultureResult(httpContext);

            result.Result.Cultures.Count.Should().Be(1);
            result.Result.Cultures.Single().Value.Should().Be("fr-CH");

            result.Result.UICultures.Count.Should().Be(1);
            result.Result.UICultures.Single().Value.Should().Be("fr-CH");
        }

        [TestMethod]
        public void DetermineProviderCultureResult_WhenContainsNoRecognisedCultures_ThenReturnsDefaultCulture()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.AcceptLanguage] = "af-AF, af;q=0.9, *;q=0.5";

            var result = _testee.DetermineProviderCultureResult(httpContext);

            result.Result.Cultures.Count.Should().Be(1);
            result.Result.Cultures.Single().Value.Should().Be(DefaultCultures.DefaultCultureInfo.Name);

            result.Result.UICultures.Count.Should().Be(1);
            result.Result.UICultures.Single().Value.Should().Be(DefaultCultures.DefaultUiCultureInfo.Name);
        }

        [TestMethod]
        public void DetermineProviderCultureResult_WhenFirstCultureNotRecognised_ThenReturnsFirstRecognisedCulture()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.AcceptLanguage] = "af-AF, af;q=0.9 en-GB;q=0.7, *;q=0.5";

            var result = _testee.DetermineProviderCultureResult(httpContext);

            result.Result.Cultures.Count.Should().Be(1);
            result.Result.Cultures.Single().Value.Should().Be("en-GB");

            result.Result.UICultures.Count.Should().Be(1);
            result.Result.UICultures.Single().Value.Should().Be(DefaultCultures.DefaultCultureInfo.Name);
        }

        [TestMethod]
        public void DetermineProviderCultureResult_WhenSpecificCultureNotRecognised_ThenReturnsFirstLocaleAgnosticCulture()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.AcceptLanguage] = "en-GB, en;q=0.7, *;q=0.5";

            var result = _testee.DetermineProviderCultureResult(httpContext);

            result.Result.Cultures.Count.Should().Be(1);
            result.Result.Cultures.Single().Value.Should().Be("en-GB");

            result.Result.UICultures.Count.Should().Be(1);
            result.Result.UICultures.Single().Value.Should().Be("en-US");
        }

        [TestMethod]
        public void DetermineProviderCultureResult_WhenLocaleIsNotSpecified_ThenReturnsFirstLocaleAgnosticCulture()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.AcceptLanguage] = "de";

            var result = _testee.DetermineProviderCultureResult(httpContext);

            result.Result.Cultures.Count.Should().Be(1);
            result.Result.Cultures.Single().Value.Should().Be("de-CH");

            result.Result.UICultures.Count.Should().Be(1);
            result.Result.UICultures.Single().Value.Should().Be("de-CH");
        }
    }
}