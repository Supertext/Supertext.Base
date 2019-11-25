using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Supertext.Base.Test.Utils.Http;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Test.Utils.Specs.Http
{
    [TestClass]
    public class UriAndResponseTests : UriAndResponseTestsBase
    {
        [TestMethod]
        public async Task UriAndResponse_For_Single_Response()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.OK;
            var testObj = GetTestObjects(1).First();
            var testUri = Random.GetUri();
            var response = new HttpResponseMessage
                               {
                                   StatusCode = statusCode,
                                   Content = new StringContent(JsonConvert.SerializeObject(testObj),
                                                               Encoding.UTF8,
                                                               "application/json")
                               };
            var uriAndResponse = new UriAndResponse(testUri, response);
            var client = GetTestClient(uriAndResponse);

            // Act
            var result = await client.GetAsync(testUri);

            // Assert
            result.StatusCode.Should().Be(statusCode);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result.Content.ReadAsStringAsync());
            testObj.Equals(resultObj).Should().BeTrue();
        }

        [TestMethod]
        public async Task UriAndResponse_For_Multiple_Responses()
        {
            // Arrange
            const HttpStatusCode statusCode0 = HttpStatusCode.Continue;
            var testObj0 = GetTestObjects(1).First();
            var testUri0 = Random.GetUri();
            var response0 = GetResponse(statusCode0, testObj0);
            var uriAndResponse0 = new UriAndResponse(testUri0, response0);

            const HttpStatusCode statusCode1 = HttpStatusCode.OK;
            var testObj1 = GetTestObjects(1).First();
            var testUri1 = Random.GetUri();
            var response1 = GetResponse(statusCode1, testObj1);
            var uriAndResponse1 = new UriAndResponse(testUri1, response1);

            var client = GetTestClient(uriAndResponse0, uriAndResponse1);

            // Act
            var result = await client.GetAsync(testUri1);

            // Assert
            result.StatusCode.Should().Be(statusCode1);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result.Content.ReadAsStringAsync());
            testObj1.Equals(resultObj).Should().BeTrue();
        }
    }
}