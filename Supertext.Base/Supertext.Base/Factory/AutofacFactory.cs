using System;

namespace Supertext.Base.Factory
{
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

        public T Create(TParam input)
        {
            return _factoryMethod(input);
        }
    }
}