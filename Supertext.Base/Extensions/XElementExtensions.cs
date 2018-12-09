using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Supertext.Base.Common;


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

        /// <summary>
        /// Returns an <c>XElement</c> containing the same elements and attributes as the specified <c>XElement</c> but with all namespaces removed.
        /// </summary>
        /// <param name="document">An XML document containing the namespaces to be removed.</param>
        /// <returns>An <c>XElement</c> without namespaces.</returns>
        public static XElement RemoveAllNamespaces(this XElement document)
        {
            Validate.NotNull(document, nameof(document));

            var namespaces = document.Attributes()
                                     .Where(attr => attr.IsNamespaceDeclaration)
                                     .Select(attr => attr.Name.LocalName)
                                     .ToList();

            var strContent = new System.Text.StringBuilder(document.ToString());
            foreach (var ns in namespaces)
            {
                strContent = strContent.Replace($"<{ns}:", "<")
                                       .Replace($"</{ns}:", "</");
            }

            var xElmnt = XElement.Parse(strContent.ToString());
            foreach (var ns in namespaces)
            {
                xElmnt.Attributes()
                        .FirstOrDefault(attr => attr.IsNamespaceDeclaration && attr.Name.LocalName == ns)
                        ?.Remove();
            }

            return xElmnt;

            /*
            XElement RemoveAllNamespaces_Recursive(XElement xmlDocument)
            {
                if (!xmlDocument.HasElements)
                {
                    var xElement = new XElement(xmlDocument.Name.LocalName) {Value = xmlDocument.Value};

                    foreach (var attribute in xmlDocument.Attributes())
                    {
                        xElement.Add(attribute);
                    }

                    return xElement;
                }

                return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(RemoveAllNamespaces));
            }

            return RemoveAllNamespaces_Recursive(document);
            */
        }
    }
}