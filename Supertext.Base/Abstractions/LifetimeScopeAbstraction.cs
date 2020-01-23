using Autofac;

namespace Supertext.Base.Abstractions
{
    internal class LifetimeScopeAbstraction : ILifetimeScopeAbstraction
    {
        private readonly ILifetimeScope _lifetimeScope;

        public LifetimeScopeAbstraction(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public TService Resolve<TService>()
        {
            return _lifetimeScope.Resolve<TService>();
        }
    }
}