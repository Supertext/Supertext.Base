using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Supertext.Base.Test.Utils.Http;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Test.Utils.Specs.Http
{
    [TestClass]
    public class RequestConditionalUriAndResponseTests : UriAndResponseTestsBase
    {
        [TestMethod]
        public async Task UriAndResponse_For_Single_Response()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.Accepted;
            var testObj = GetTestObjects(1).First();
            var testUri = Random.GetUri();
            var response = GetResponse(statusCode, testObj);
            var uriAndResponse = new RequestConditionalUriAndResponse(testUri,
                                                                      response,
                                                                      request => request.Method == HttpMethod.Post);
            var client = GetTestClient(uriAndResponse);

            // Act
            // - should return NotImplemented because it's a GET request
            var result = await client.GetAsync(testUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotImplemented);

            // Act
            // - should return expected result because it's a POST request
            result = await client.PostAsync(testUri, new StringContent(String.Empty));

            // Assert
            result.StatusCode.Should().Be(statusCode);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result.Content.ReadAsStringAsync());
            testObj.Equals(resultObj).Should().BeTrue();
        }

        [TestMethod]
        public async Task UriAndResponse_For_Multiple_Responses()
        {
            // Arrange
            const HttpStatusCode statusCode0 = HttpStatusCode.Accepted;
            var testObj0 = GetTestObjects(1).First();
            var testUri0 = Random.GetUri();
            var response0 = GetResponse(statusCode0, testObj0);
            var uriAndResponse0 = new RequestConditionalUriAndResponse(testUri0,
                                                                       response0,
                                                                       request => request.Method == HttpMethod.Get);

            const HttpStatusCode statusCode1 = HttpStatusCode.Accepted;
            var testObj1 = GetTestObjects(1).First();
            var testUri1 = Random.GetUri();
            var response1 = GetResponse(statusCode1, testObj1);
            var uriAndResponse1 = new RequestConditionalUriAndResponse(testUri1,
                                                                       response1,
                                                                       request => request.Method == HttpMethod.Post);
            var client = GetTestClient(uriAndResponse0, uriAndResponse1);

            // Act
            var result = await client.GetAsync(testUri0);

            // Assert
            result.StatusCode.Should().Be(statusCode0);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result.Content.ReadAsStringAsync());
            testObj0.Equals(resultObj).Should().BeTrue();

            // Act
            result = await client.PostAsync(testUri1, new StringContent(String.Empty));

            // Assert
            result.StatusCode.Should().Be(statusCode0);
            resultObj = JsonConvert.DeserializeObject<TestClass>(await result.Content.ReadAsStringAsync());
            testObj1.Equals(resultObj).Should().BeTrue();
        }
    }
}