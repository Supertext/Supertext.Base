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

    internal class AutofacFactory<TParam1, Param2, T> : IFactory<TParam1, Param2, T>
    {
        private readonly Func<TParam1, Param2, T> _factoryMethod;

        public AutofacFactory(Func<TParam1, Param2, T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public T Create(TParam1 param1, Param2 param2)
        {
            return _factoryMethod(param1, param2);
        }
    }
}