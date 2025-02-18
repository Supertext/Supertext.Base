using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    public class SqlResultConverter : ISqlResultConverter
    {
        public TEntity InterpretUtcDates<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return null;
            }

            if (entity is IDictionary<string, object> dictionary)
            {
                return InterpretDictionaryUtcDates(dictionary) as TEntity;
            }

            return InterpretUtcDatesRecursive(entity);
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

        private IDictionary<string, object> InterpretDictionaryUtcDates(IDictionary<string, object> row)
        {
            return row.ToDictionary(field => field.Key,
                                    field => field.Value is DateTime date ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : field.Value);
        }

        private TEntity InterpretUtcDatesRecursive<TEntity>(TEntity entity) where TEntity : class
        {
            var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    ProcessDateTimeProperty(property, entity);
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var nestedObject = property.GetValue(entity);
                    if (nestedObject != null)
                    {
                        var updatedNestedObject = InterpretUtcDatesRecursive(nestedObject);
                        property.SetValue(entity, updatedNestedObject);
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                {
                    if (property.GetValue(entity) is IEnumerable collection)
                    {
                        foreach (var item in collection)
                        {
                            if (item is DateTime)
                            {
                                ProcessDateTimeProperty(property, entity);
                            }
                            else if (item != null && item.GetType().IsClass && item.GetType() != typeof(string))
                            {
                                InterpretUtcDatesRecursive(item);
                            }
                        }
                    }
                }
            }

            return entity;
        }

        private void ProcessDateTimeProperty<TEntity>(PropertyInfo property, TEntity entity)
        {
            var currentValue = property.GetValue(entity);

            if (currentValue != null && property.PropertyType == typeof(DateTime))
            {
                var dateTime = (DateTime)currentValue;
                if (dateTime.Kind != DateTimeKind.Utc)
                {
                    var utcValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    property.SetValue(entity, utcValue);
                }
            }
            else if (property.PropertyType == typeof(DateTime?))
            {
                var nullableDateTime = (DateTime?)currentValue;
                if (nullableDateTime.HasValue && nullableDateTime.Value.Kind != DateTimeKind.Utc)
                {
                    var utcValue = DateTime.SpecifyKind(nullableDateTime.Value, DateTimeKind.Utc);
                    property.SetValue(entity, utcValue);
                }
            }
        }
    }
}