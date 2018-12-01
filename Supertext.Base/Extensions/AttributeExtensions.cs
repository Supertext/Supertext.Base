using Supertext.Base.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Supertext.Base.Extensions
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// Returns the values of attributes for a class.
        /// </summary>
        /// <example>
        /// Read System.ComponentModel Description Attribute from class 'MyClass':
        ///     var Attribute = typeof(MyClass).GetAttributeValues((DescriptionAttribute d) => d.Description);
        /// </example>
        /// <param name="type">The class <see cref="Type"/> which contains the method.</param>
        /// <param name="valueSelector">Attribute type and property to get.</param>
        /// <param name="inherit"><c>true</c> to search this class's inheritance chain to find the attributes; otherwise, <c>false</c>.</param>
        public static IEnumerable<TValue> GetAttributeValues<TAttribute, TValue>(this Type type,
                                                                                 Func<TAttribute, TValue> valueSelector,
                                                                                 bool inherit = false) where TAttribute : Attribute
        {
            var attrs = type.GetClassLevelAttributes<TAttribute>(inherit);

            foreach (var attr in attrs)
            {
                yield return valueSelector(attr);
            }
        }


        /// <summary>
        /// Returns the values of method attributes for any method in a class.
        /// </summary>
        /// <example>
        /// Read System.ComponentModel Description Attribute from method 'MyMethodName(string)' in class 'MyClass':
        ///     var Attribute = typeof(MyClass).GetAttributeValues("MyMethodName", (DescriptionAttribute d) => d.Description);
        /// </example>
        /// <param name="type">The class <see cref="Type"/> which contains the method.</param>
        /// <param name="methodName">Name of the method in the class.</param>
        /// <param name="valueSelector">Attribute type and property to get.</param>
        /// <param name="considerClassLevel">Indicates whether class-level attributes should also be considered.</param>
        /// <param name="inherit">
        /// <para>Set to <c>true</c> to search this method's inheritance chain to find the attributes; otherwise, <c>false</c>.</para>
        /// <para>The inheritance refers to the method itself, not the class; <see cref="inherit"/> parameter is meaningless unless the method is overriding a base method.</para>
        /// </param>
        /// <exception cref="ArgumentException">No method could be obtained using the specified argument.</exception>
        /// <exception cref="AttributeNotFoundException">The specified method is not decorated with the specified attribute.</exception>
        /// <exception cref="AmbiguousMatchException">More than one method is found with the specified name.</exception>
        public static IEnumerable<TValue> GetAttributeValues<TAttribute, TValue>(this Type type,
                                                                                 string methodName,
                                                                                 Func<TAttribute, TValue> valueSelector,
                                                                                 bool considerClassLevel = false,
                                                                                 bool inherit = false) where TAttribute : Attribute
        {
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                throw new ArgumentException($"No method could be obtained using the specified {nameof(methodName)} argument.");
            }

            var attrs = methodInfo.GetCustomAttributes(typeof(TAttribute), inherit) as TAttribute[];

            if (considerClassLevel)
            {
                var attrs2 = attrs.ToList();
                attrs2.AddRange(type.GetClassLevelAttributes<TAttribute>(inherit));
                attrs = attrs2.ToArray();
            }

            if (attrs == null || !attrs.Any())
            {
                throw new AttributeNotFoundException(methodInfo, valueSelector.GetMethodInfo().Name);
            }

            foreach (var attr in attrs)
            {
                yield return valueSelector(attr);
            }
        }


        /// <summary>
        /// Returns the values of method attributes for any method in a class.
        /// </summary>
        /// <example>
        /// Read System.ComponentModel Description Attribute from method 'MyMethodName(string)' in class 'MyClass':
        ///     var Attribute = typeof(MyClass).GetAttributeValues("MyMethodName", new [] { System.String }, (DescriptionAttribute d) => d.Description);
        /// </example>
        /// <param name="type">The class <see cref="Type"/> which contains the method.</param>
        /// <param name="methodName">Name of the method in the class.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects representing the number, order and type of the parameters for the method to get -or- An empty array of <see cref="Type"/> objects (as provided by the <see cref="Type.EmptyTypes"/> field) to get a method that takes no parameters.</param>
        /// <param name="valueSelector">Attribute type and property to get.</param>
        /// <param name="considerClassLevel">Indicates whether class-level attributes should also be considered.</param>
        /// <param name="inherit">
        /// <para>Set to <c>true</c> to search this method's inheritance chain to find the attributes; otherwise, <c>false</c>.</para>
        /// <para>The inheritance refers to the method itself, not the class; <see cref="inherit"/> parameter is meaningless unless the method is overriding a base method.</para>
        /// </param>
        /// <exception cref="ArgumentException">Thrown if no method could be obtained using the specified arguments.</exception>
        /// <exception cref="AttributeNotFoundException">Thrown if the specified method is not decorated with the specified attribute.</exception>
        public static IEnumerable<TValue> GetAttributeValues<TAttribute, TValue>(this Type type,
                                                                                 string methodName,
                                                                                 Type[] parameterTypes,
                                                                                 Func<TAttribute, TValue> valueSelector,
                                                                                 bool considerClassLevel = false,
                                                                                 bool inherit = false) where TAttribute : Attribute
        {
            var methodInfo = type.GetMethod(methodName, parameterTypes);
            if (methodInfo == null)
            {
                throw new ArgumentException($"No method could be obtained using the specified combination of {nameof(methodName)} and {nameof(parameterTypes)} arguments.");
            }

            var attrs = methodInfo.GetCustomAttributes(typeof(TAttribute), inherit) as TAttribute[];

            if (considerClassLevel)
            {
                var attrs2 = attrs.ToList();
                attrs2.AddRange(type.GetClassLevelAttributes<TAttribute>(inherit));
                attrs = attrs2.ToArray();
            }

            if (attrs == null || !attrs.Any())
            {
                throw new AttributeNotFoundException(methodInfo, valueSelector.GetMethodInfo().Name);
            }

            foreach (var attr in attrs)
            {
                yield return valueSelector(attr);
            }
        }


        private static IEnumerable<TAttribute> GetClassLevelAttributes<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {
            var attrs = type.GetCustomAttributes(typeof(TAttribute), inherit) as TAttribute[];

            if (attrs == null || !attrs.Any())
            {
                throw new AttributeNotFoundException(type.GetTypeInfo(), typeof(TAttribute).Name);
            }

            return attrs;
        }
    }
}