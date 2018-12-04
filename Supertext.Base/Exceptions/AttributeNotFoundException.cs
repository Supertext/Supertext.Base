using System;
using System.Reflection;


namespace Supertext.Base.Exceptions
{
    public class AttributeNotFoundException : Exception
    {
        public AttributeNotFoundException()
        { }


        public AttributeNotFoundException(TypeInfo typeInfo, string attributeName) : base($"\"{typeInfo.DeclaringType}.{typeInfo.Name}\" has not been decorated with \"{attributeName}\".")
        { }


        public AttributeNotFoundException(MemberInfo memberInfo, string attributeName) : base($"\"{memberInfo.DeclaringType}.{memberInfo.Name}\" has not been decorated with \"{attributeName}\".")
        { }
    }
}