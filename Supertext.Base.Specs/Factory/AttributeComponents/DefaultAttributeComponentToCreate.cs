namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    [CustomComponentKey("Default", IsDefault = true)]
    public class DefaultAttributeComponentToCreate : IAttributeComponentToCreate
    {
        public static string ReturnValue = "default";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}