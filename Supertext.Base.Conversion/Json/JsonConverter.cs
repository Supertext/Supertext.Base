using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Supertext.Base.Conversion.Json
{
    internal class JsonConverter : IJsonConverter
    {
        public TResult Deserialize<TResult>(string content)
        {
            return JsonConvert.DeserializeObject<TResult>(content);
        }

        public object DeserializeObject(string content)
        {
            return JsonConvert.DeserializeObject(content);
        }

        public string Serialize<TObject>(TObject jsonObject)
        {
            var settings = new JsonSerializerSettings()
                           {
                               ContractResolver = new CamelCasePropertyNamesContractResolver()
                           };
            return JsonConvert.SerializeObject(jsonObject, Formatting.None, settings);
        }

        public string GetTokenValue(string json, string tokenName)
        {
            var s = JObject.Parse(json);
            var token = s.SelectToken(tokenName);

            return token?.ToString();
        }

        public string ReplaceTokenValue(string parent, string tokenName, Func<string, string> mapValueCallback)
        {
            var parentJson = JObject.Parse(parent);

            parentJson[tokenName] = mapValueCallback((string)parentJson.SelectToken($"$.{tokenName}"));

            return parentJson.ToString();
        }
    }
}