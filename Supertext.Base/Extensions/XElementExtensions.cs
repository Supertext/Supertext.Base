using System;
using System.ComponentModel;
using System.Xml.Linq;


namespace Supertext.Base.Extensions
{
    public static class XElementExtensions
    {
        /// <summary>
        /// <para>Returns the value of the specified attribute, if the attribute is found.</para>
        /// <para>If the attribute is not found then either the optional parameter <see cref="defaultValue"/> will be returned, if specified, or the
        /// default value of the generic type will be returned.</para>
        /// </summary>
        /// <typeparam name="T">The generic type of the return value.</typeparam>
        /// <param name="element">An instance of <see cref="XElement"/> which is expected to contain an attribute with the specified <see cref="attrName"/>.</param>
        /// <param name="attrName">The name of the sought attribute.</param>
        /// <param name="defaultValue">
        /// <para>The optional default value which will be returned if the specified attribute is not found.</para>
        /// <para>If specified, the declaration of <see cref="T"/> after the method call is not required.</para>
        /// </param>
        /// <example>
        /// var myInt = elmnt.GetAttrValue{int}("id");
        /// </example>
        /// <example>
        /// var myInt = elmnt.GetAttrValue("id", -1);
        /// </example>
        public static T GetAttrValue<T>(this XElement element,
                                        string attrName,
                                        T defaultValue = default(T))
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Cannot be null.");
            }

            if (String.IsNullOrWhiteSpace(attrName))
            {
                throw new ArgumentException("Invalid attribute name.", nameof(attrName));
            }

            var attr = element.Attribute(attrName);
            if (attr != null)
            {
                // we need TrimEnd here because SQL Server has a knack of padding values to the DB field's length
                return (T) TypeDescriptor.GetConverter(typeof(T))
                                         .ConvertFromInvariantString(attr.Value.TrimEnd());
            }

            return defaultValue;
        }
    }
}