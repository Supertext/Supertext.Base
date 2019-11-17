using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Supertext.Base.Test.Utils.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Test.Utils.Specs.Http
{
    [TestClass]
    public class ContentConditionalUriAndResponseTests : UriAndResponseTestsBase
    {
        [TestMethod]
        public async Task UriAndResponse_For_Single_Response()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.Accepted;
            var testRequestObj = GetTestObjects(1).First();
            var testResponseObj = GetTestObjects(1).First();
            var testUri = Random.GetUri();
            var response = GetResponse(statusCode, testResponseObj);
            var uriAndResponse = new ContentConditionalUriAndResponse<TestClass>(testUri,
                                                                                 response,
                                                                                 request => request.IntProp == testRequestObj.IntProp);
            var client = GetTestClient(uriAndResponse);

            // Act
            var result = await client.PostAsync(testUri, GetAsStringContent(testRequestObj));

            // Assert
            result.StatusCode.Should().Be(statusCode);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result.Content.ReadAsStringAsync());
            testResponseObj.Equals(resultObj).Should().BeTrue();
        }

        [TestMethod]
        public async Task UriAndResponse_Throws_Excpn_For_All_Http_Verbs_Except_Patch_Post_And_Put()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.Accepted;
            var testRequestObj = GetTestObjects(1).First();
            var testResponseObj = GetTestObjects(1).First();
            var testUri = Random.GetUri();
            var response = GetResponse(statusCode, testResponseObj);
            var uriAndResponse = new ContentConditionalUriAndResponse<TestClass>(testUri,
                                                                                 response,
                                                                                 request => request.IntProp == testRequestObj.IntProp);
            var client = GetTestClient(uriAndResponse);

            var permittedMethods = new List<HttpMethod>
                                       {
                                           HttpMethod.Patch,
                                           HttpMethod.Post,
                                           HttpMethod.Put
                                       };

            // Act
            foreach (var httpMethod in typeof(HttpMethod).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                                         .Where(propInfo => propInfo.PropertyType == typeof(HttpMethod))
                                                         .Select(propInfo => (HttpMethod) propInfo.GetValue(null))
                                                         .Where(httpMethod => !permittedMethods.Contains(httpMethod)))
            {
                try
                {
                    await client.SendAsync(new HttpRequestMessage(httpMethod, testUri));
                }
                catch (InvalidOperationException exception) when (exception.Message.StartsWith($"Invalid HTTP method: {httpMethod.Method}."))
                {
                    continue;
                }

                Assert.Fail($"Http method {httpMethod.Method} was unexpectedly permitted.");
            }
        }

        [TestMethod]
        public async Task UriAndResponse_For_Multiple_Responses()
        {
            // Arrange
            const HttpStatusCode statusCode0 = HttpStatusCode.Accepted;
            var testRequestObj0 = GetTestObjects(1).First();
            var testResponseObj0 = GetTestObjects(1).First();
            var testUri = Random.GetUri();
            var response0 = GetResponse(statusCode0, testResponseObj0);
            var uriAndResponse0 = new ContentConditionalUriAndResponse<TestClass>(testUri,
                                                                                  response0,
                                                                                  request => request.IntProp == testRequestObj0.IntProp);

            const HttpStatusCode statusCode1 = HttpStatusCode.Created;
            var testRequestObj1 = GetTestObjects(1).First();
            var testResponseObj1 = GetTestObjects(1).First();
            var response1 = GetResponse(statusCode1, testResponseObj1);
            var uriAndResponse1 = new ContentConditionalUriAndResponse<TestClass>(testUri,
                                                                                  response1,
                                                                                  request => request.IntProp == testRequestObj1.IntProp);

            var client = GetTestClient(uriAndResponse0, uriAndResponse1);

            // Act
            var result0 = await client.PostAsync(testUri, GetAsStringContent(testRequestObj0));
            var result1 = await client.PostAsync(testUri, GetAsStringContent(testRequestObj1));

            // Assert
            result0.StatusCode.Should().Be(statusCode0);
            var resultObj = JsonConvert.DeserializeObject<TestClass>(await result0.Content.ReadAsStringAsync());
            testResponseObj0.Equals(resultObj).Should().BeTrue();

            result1.StatusCode.Should().Be(statusCode1);
            resultObj = JsonConvert.DeserializeObject<TestClass>(await result1.Content.ReadAsStringAsync());
            testResponseObj1.Equals(resultObj).Should().BeTrue();
        }
    }
}