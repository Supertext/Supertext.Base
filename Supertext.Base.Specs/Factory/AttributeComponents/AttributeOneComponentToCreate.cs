namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    [CustomComponentKey("one")]
    public class AttributeOneComponentToCreate : IAttributeComponentToCreate
    {
        public static string ReturnValue = "one";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}