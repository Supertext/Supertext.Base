using Supertext.Base.Factory;

namespace Supertext.Base.Specs.Factory.CustomKeyComponents
{
    public class ComponentDispatcher : IComponentDispatcher
    {
        private readonly IKeyFactory<ComponentKeyType, IKeyComponentToCreate> _factory;

        public ComponentDispatcher(IKeyFactory<ComponentKeyType, IKeyComponentToCreate> factory)
        {
            _factory = factory;
        }

        public string DoSomething(ComponentKeyType keyType)
        {
            return _factory.CreateComponent(keyType).DoSomething();
        }
    }
}