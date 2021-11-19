using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Test.Utils.Http.Specs
{
    [TestClass]
    public class OrderedUriAndResponseTests : UriAndResponseTestsBase
    {
        [TestMethod]
        public async Task UriAndResponse_WhenConfiguredResponseCountLessThanRequestCount_ReturnsNotImplemented()
        {
            // Arrange
            var testUri = Random.GetUri();

            const HttpStatusCode statusCode0 = HttpStatusCode.Continue;
            var testObj0 = GetTestObjects(1).First();
            var response0 = GetResponse(statusCode0, testObj0);

            var orderedUriAndResponse = new OrderedUriAndResponse(testUri, new List<HttpResponseMessage> { response0 });

            var client = GetTestClient(orderedUriAndResponse);

            // Act
            var result0 = await client.GetAsync(testUri);
            var result1 = await client.GetAsync(testUri);

            // Assert
            result0.StatusCode.Should().Be(statusCode0);
            result1.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        public async Task OrderedUriAndResponse_WhenConfiguredResponseCountGreaterThanOrEqualToRequestCount_ReturnsConfiguredResponse()
        {
            // Arrange
            var testUri = Random.GetUri();

            const HttpStatusCode statusCode0 = HttpStatusCode.Continue;
            var testObj0 = GetTestObjects(1).First();
            var response0 = GetResponse(statusCode0, testObj0);

            const HttpStatusCode statusCode1 = HttpStatusCode.OK;
            var testObj1 = GetTestObjects(1).First();
            var response1 = GetResponse(statusCode1, testObj1);

            var orderedUriAndResponse = new OrderedUriAndResponse(testUri,
                                                                  new List<HttpResponseMessage>
                                                                  {
                                                                      response0,
                                                                      response1
                                                                  });

            var client = GetTestClient(orderedUriAndResponse);

            // Act
            var result0 = await client.GetAsync(testUri);
            var result1 = await client.GetAsync(testUri);

            // Assert
            result0.StatusCode.Should().Be(statusCode0);
            result1.StatusCode.Should().Be(statusCode1);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result1.Content.ReadAsStringAsync());
            testObj1.Equals(resultObj).Should().BeTrue();
        }
    }
}