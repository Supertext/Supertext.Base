using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    public class SqlResultConverter : ISqlResultConverter
    {
        public IDictionary<string, object> InterpretUtcDates(IDictionary<string, object> row)
        {
            return row.ToDictionary(
                field => field.Key,
                field => field.Value is DateTime date ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : field.Value);
        }

        public IDictionary<string, object> DecodeStructure(IDictionary<string, object> row)
        {
            var result = new Dictionary<string, object>(row);
            foreach (var key in row.Keys)
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

                        parent = (Dictionary<string, object>)parent[subKey];
                    }
                    parent.Add(subKeys[subKeys.Length - 1], value);
                    result.Remove(key);
                }
                else
                {
                    result[plainKey] = value;
                }
            }
            return result;
        }

    }
}