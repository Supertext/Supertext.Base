using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Supertext.Base.Test.Utils.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HttpMessageHandler = Supertext.Base.Test.Utils.Http.HttpMessageHandler;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Test.Utils.Specs.Http
{
    [TestClass]
    public abstract class UriAndResponseTestsBase
    {
        protected static StringContent GetAsStringContent(TestClass testObj)
        {
            return new StringContent(JsonConvert.SerializeObject(testObj),
                                     Encoding.UTF8,
                                     "application/json");
        }

        protected static HttpClient GetTestClient(params UriAndResponse[] urisAndResponses)
        {
            return GetTestClient(urisAndResponses.ToList());
        }

        protected static HttpClient GetTestClient(List<UriAndResponse> urisAndResponses)
        {
            var messageHandler = new HttpMessageHandler(urisAndResponses);
            return new HttpClient(messageHandler);
        }

        protected static IEnumerable<TestClass> GetTestObjects(int count)
        {
            IEnumerable<TestClass> ReturnFromStream()
            {
                while (true)
                {
                    yield return new TestClass
                                     {
                                         GuidProp = Guid.NewGuid(),
                                         IntProp = Random.GetInt(),
                                         StringProp = Random.GetString(10)
                                     };
                }
            }

            return ReturnFromStream().Take(count).ToList();
        }

        protected static HttpResponseMessage GetResponse(HttpStatusCode statusCode, TestClass testObj)
        {
            return new HttpResponseMessage
                       {
                           StatusCode = statusCode,
                           Content = GetAsStringContent(testObj)
                       };
        }
    }
}