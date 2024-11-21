using System;
using System.Collections.Generic;
using System.Linq;

namespace Supertext.Base.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// Check if the type is IDictionary&lt;S, T&gt;, or implements IDictionary&lt;S, T&gt;, for any S and T.
    /// </summary>
    public static bool IsDictionary(this Type type)
    {
        // Check if the type implements IDictionary<S, T> for any S and T
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
               || type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
    }
}