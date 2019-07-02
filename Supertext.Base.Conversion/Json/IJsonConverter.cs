using System;

namespace Supertext.Base.Conversion.Json
{
    public interface IJsonConverter
    {
        TResult Deserialize<TResult>(string content);

        object DeserializeObject(string content);

        string Serialize<TObject>(TObject jsonObject);

        string GetTokenValue(string json, string tokenName);

        string ReplaceTokenValue(string parent, string tokenName, Func<string, string> mapValueCallback);
    }
}