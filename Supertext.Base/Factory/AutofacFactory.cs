using System;
using Autofac;

namespace Supertext.Base.Factory
{
    internal class AutofacFactory : IFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public T Create<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }

    internal class AutofacFactory<T> : IFactory<T>
    {
        private readonly Func<T> _factoryMethod;

        public AutofacFactory(Func<T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public T Create()
        {
            return _factoryMethod();
        }
    }

    internal class AutofacFactory<TParam, T> : IFactory<TParam, T>
    {
        private readonly Func<TParam, T> _factoryMethod;

        public AutofacFactory(Func<TParam, T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public T Create(TParam param)
        {
            return _factoryMethod(param);
        }
    }

    internal class AutofacFactory<TParam1, TParam2, T> : IFactory<TParam1, TParam2, T>
    {
        private readonly Func<TParam1, TParam2, T> _factoryMethod;

        public AutofacFactory(Func<TParam1, TParam2, T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public T Create(TParam1 param1, TParam2 param2)
        {
            return _factoryMethod(param1, param2);
        }
    }

    internal class AutofacFactory<TParam1, TParam2, TParam3, T> : IFactory<TParam1, TParam2, TParam3, T>
    {
        private readonly Func<TParam1, TParam2, TParam3, T> _factoryMethod;

        public AutofacFactory(Func<TParam1, TParam2, TParam3, T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public T Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _factoryMethod(param1, param2, param3);
        }
    }
}