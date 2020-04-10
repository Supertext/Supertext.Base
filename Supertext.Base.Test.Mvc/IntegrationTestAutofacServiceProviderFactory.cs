using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Test.Mvc
{
    internal class IntegrationTestAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> _registerMockedComponentsDelegate;

        public IntegrationTestAutofacServiceProviderFactory(Action<ContainerBuilder> registerMockedComponentsDelegate = null)
        {
            _registerMockedComponentsDelegate = registerMockedComponentsDelegate;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            _registerMockedComponentsDelegate?.Invoke(containerBuilder);
            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}