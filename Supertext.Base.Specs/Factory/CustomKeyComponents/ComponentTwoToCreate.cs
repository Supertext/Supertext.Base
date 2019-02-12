namespace Supertext.Base.Specs.Factory.CustomKeyComponents
{
    public class ComponentTwoToCreate : IKeyComponentToCreate
    {
        public static string ReturnValue = "two";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}