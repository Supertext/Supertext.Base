using Supertext.Base.Factory;

namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    public class AttributeComponentDispatcher : IAttributeComponentDispatcher
    {

        private readonly IKeyFactory<string, IAttributeComponentToCreate> _factory;

        public AttributeComponentDispatcher(IKeyFactory<string, IAttributeComponentToCreate> factory)
        {
            _factory = factory;
        }

        public string DoSomething(string key)
        {
            return _factory.CreateComponent(key).DoSomething();
        }
    }
}