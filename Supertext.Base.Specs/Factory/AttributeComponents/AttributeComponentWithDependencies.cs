using Supertext.Base.Factory;

namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    [CustomComponentKey("dependencies")]
    public class AttributeComponentWithDependencies : IAttributeComponentToCreate
    {
        private readonly IFactory<ISomeDependencies> _factory;

        public AttributeComponentWithDependencies(IFactory<ISomeDependencies> factory)
        {
            _factory = factory;
        }

        public static string ReturnValue = "dependencies";
        public string DoSomething()
        {
            return ReturnValue;
        }
    }
}