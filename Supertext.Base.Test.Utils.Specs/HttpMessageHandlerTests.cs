using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Test.Utils.Http.Specs
{
    [TestClass]
    public class HttpMessageHandlerTests : UriAndResponseTestsBase
    {
        [TestMethod]
        public async Task Returns_NotImplemented_When_No_UriAndResponses_Configured()
        {
            // Arrange
            var client = GetTestClient();

            // Act
            var response = await client.GetAsync(Random.GetUri());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        public async Task Returns_NotImplemented_When_No_Matching_UriAndResponses_Configured()
        {
            // Arrange
            const HttpStatusCode statusCode0 = HttpStatusCode.Continue;
            var testObj = GetTestObjects(1).First();
            var testUri = Random.GetUri();
            var response = GetResponse(statusCode0, testObj);
            var uriAndResponse = new UriAndResponse(testUri, response);

            var unrecognisedUri = Random.GetUri();

            var client = GetTestClient(uriAndResponse);

            // Act
            var result = await client.GetAsync(unrecognisedUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
    }
}