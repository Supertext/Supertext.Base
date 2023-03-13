using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    public class SqlResultConverter : ISqlResultConverter
    {
        public void InterpretUtcDates(IEnumerable<dynamic> results)
        {
            foreach (IDictionary<string, object> result in results)
            {
                foreach (var key in result.Keys)
                {
                    var value = result[key];
                    if (value is DateTime date)
                    {
                        result[key] = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                    }
                }
            }
        }

        public void DecodeStructure(IEnumerable<dynamic> results)
        {
            foreach (IDictionary<string, object> result in results)
            {
                foreach (var key in result.Keys.ToList())
                {
                    var plainKey = key;
                    var value = result[key];
                    if (value == null)
                    {
                        result.Remove(key);
                        continue;
                    }
                    if (key.EndsWith("_json_"))
                    {
                        plainKey = key.Substring(0, key.Length - 6);
                        value = JsonSerializer.Deserialize<dynamic>((string)value)
                                ?? new object[] { };
                        result.Remove(key);
                    }
                    if (plainKey.Contains('.'))
                    {
                        var parent = result;
                        var subKeys = plainKey.Split('.');
                        foreach (var subKey in subKeys.Take(subKeys.Length - 1))
                        {
                            if (!parent.ContainsKey(subKey))
                            {
                                parent.Add(subKey, new Dictionary<string, object>());
                            }

                            parent = (IDictionary<string, object>)parent[subKey];
                        }

                        parent.Add(subKeys[subKeys.Length - 1], value);
                        result.Remove(key);
                    }
                    else
                    {
                        result[plainKey] = value;
                    }
                }
            }
        }

    }
}