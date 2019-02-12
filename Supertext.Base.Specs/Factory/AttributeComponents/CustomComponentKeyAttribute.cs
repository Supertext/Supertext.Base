using Supertext.Base.Factory;

namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    public class CustomComponentKeyAttribute : ComponentKeyAttribute
    {
        public CustomComponentKeyAttribute(string key)
            : base(key)
        {
        }
    }
}