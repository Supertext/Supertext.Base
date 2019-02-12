namespace Supertext.Base.Specs.Factory.CustomKeyComponents
{
    public class DefaultComponentToCreate : IKeyComponentToCreate
    {
        public static string ReturnValue = "default";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}