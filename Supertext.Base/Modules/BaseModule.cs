﻿using Autofac;
using Supertext.Base.Factory;

namespace Supertext.Base.Modules
{
    public class BaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterFactories(builder);
        }

        private void RegisterFactories(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(AutofacFactory<>)).As(typeof(IFactory<>));
            builder.RegisterGeneric(typeof(AutofacFactory<,>)).As(typeof(IFactory<,>));
        }
    }
}