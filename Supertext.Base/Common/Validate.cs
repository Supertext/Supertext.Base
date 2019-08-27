using System;
using System.Collections.Generic;
using System.Linq;

namespace Supertext.Base.Common
{
    public static class Validate
    {
        private const string EmptyEnumerableMessage = "enumerable is empty";
        private const string BlankStringMessage = "string is null or whitespace";
        private const string EmptyStringMessage = "string is empty";

        /// <summary>
        /// throws ArgumentException if expression is false.
        /// </summary>
        /// <param name="expression"></param>
        /// <exception cref="ArgumentException">thrown if expression is false</exception>
        public static void IsTrue(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException("expression is false");
            }
        }

        /// <summary>
        /// throws ArgumentException(message) if expression is false.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="message">for exception</param>
        /// <exception cref="ArgumentException">thrown if expression is false</exception>
        public static void IsTrue(bool expression, string message)
        {
            if (!expression)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// throws ArgumentException if given enumerable is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <exception cref="ArgumentException">thrown if enumerable is empty</exception>
        public static void NotEmpty<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null || !enumerable.Any())
            {
                throw new ArgumentException(EmptyEnumerableMessage);
            }
        }

        /// <summary>
        /// throws ArgumentException(message) if given enumerable is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentException">thrown if enumerable is empty</exception>
        public static void NotEmpty<T>(ICollection<T> enumerable, string parameterName)
        {
            if (enumerable == null || !enumerable.Any())
            {
                throw new ArgumentException(EmptyEnumerableMessage, parameterName);
            }
        }

        /// <summary>
        /// throws ArgumentException, if given string is blank
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException">thrown, if string is blank</exception>
        [Obsolete("Use NotNullOrWhitespace instead")]
        public static void NotBlank(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException(BlankStringMessage);
            }
        }

        /// <summary>
        /// throws ArgumentException(message), if given string is blank
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentException">thrown, if string is blank</exception>
        [Obsolete("Use NotNullOrWhitespace instead")]
        public static void NotBlank(string str, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException(BlankStringMessage, parameterName);
            }
        }

        /// <summary>
        /// throws ArgumentException, if string is empty
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException">thrown, if string is empty</exception>
        public static void NotEmpty(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(EmptyStringMessage);
            }
        }

        /// <summary>
        /// throws ArgumentException(message), if string is empty
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paramterName"></param>
        /// <exception cref="ArgumentException">thrown, if string is empty</exception>
        public static void NotEmpty(string str, string paramterName)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(EmptyStringMessage, paramterName);
            }
        }

        /// <summary>
        /// throws ArgumentException, if object is null
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentException">thrown, if object is null</exception>
        public static void NotNull(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// throws ArgumentException(message), if object is null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentException">thrown, if object is null</exception>
        public static void NotNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// throws ArgumentException, if given string is null or whitespace
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException">thrown, if string is null or whitespace</exception>
        public static void NotNullOrWhitespace(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException(BlankStringMessage);
            }
        }

        /// <summary>
        /// throws ArgumentException(message), if given string is null or whitespace
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentException">thrown, if string is null or whitespace</exception>
        public static void NotNullOrWhitespace(string str, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException(BlankStringMessage, parameterName);
            }
        }
    }
}