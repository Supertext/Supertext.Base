using System;

namespace Supertext.Base.Factory
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentKeyAttribute : Attribute
    {
        public ComponentKeyAttribute(string key)
        {
            Key = key;
        }

        public ComponentKeyAttribute(object key)
        {
            Key = key;
        }

        public object Key { get; private set; }

        /// <summary>
        /// Set true for the Default-Handling-Component
        /// </summary>
        public bool IsDefault { get; set; }
    }
}