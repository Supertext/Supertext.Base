using System;
using System.Linq;
using System.Reflection;

namespace Supertext.Base.Test.Utils.Specs.Http
{
    /// <summary>
    /// This is a simple class which can be used for serialising and deserialising.
    /// </summary>
    public class TestClass
    {
        public Guid GuidProp { get; set; }

        public int IntProp { get; set; }

        public string StringProp { get; set; }

        public bool Equals(TestClass obj)
        {
            return !(from propInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     let thisValue = propInfo.GetValue(this)
                     let otherValue = propInfo.GetValue(obj)
                     where !thisValue.Equals(otherValue)
                     select thisValue).Any();
        }
    }
}