namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    [CustomComponentKey("two")]
    public class AttributeTwoComponentToCreate : IAttributeComponentToCreate
    {
        public static string ReturnValue = "two";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}