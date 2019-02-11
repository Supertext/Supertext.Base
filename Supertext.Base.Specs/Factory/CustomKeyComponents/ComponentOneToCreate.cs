namespace Supertext.Base.Specs.Factory.CustomKeyComponents
{
    public class ComponentOneToCreate : IKeyComponentToCreate
    {

        public static string ReturnValue = "one";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}