using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using Autofac.Core;
using Supertext.Base.Common;
using Supertext.Base.Exceptions;
using Supertext.Base.Modularity;

[assembly: InternalsVisibleTo("Supertext.Base.Specs")]
namespace Supertext.Base.Factory
{
    internal sealed class AutofacKeyFactory<TKey, T> : IKeyFactory<TKey, T>
    {
        private readonly IComponentContext _componentContext;
        private readonly Lazy<IDictionary<TKey, IComponentRegistration>> _registrations;
        private IComponentRegistration _defaultComponentAttributeRegistration;

        public AutofacKeyFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
            _registrations = new Lazy<IDictionary<TKey, IComponentRegistration>>(CollectRegistrations);
        }

        public bool ComponentExists(TKey key)
        {
            Validate.NotNull(key, "key");

            if (ExistsKeyService(key))
            {
                return true;
            }

            return ExistsComponentAttributeService(key);
        }

        public T CreateComponent(TKey key)
        {
            Validate.NotNull(key, "key");

            if (ExistsKeyService(key))
            {
                return ResolveKeyService(key);
            }
            if (ExistsDefaultKeyService())
            {
                return ResolveDefaultKeyService();
            }
            if (ExistsComponentAttributeService(key))
            {
                return ResolveComponentAttributeService(key);
            }
            if (ExistsDefaultComponentAttributeService())
            {
                return ResolveDefaultComponentAttributeService();
            }
            throw new KeyNotFoundException($"No component registered with key '{key}'");
        }

        private T ResolveKeyService(TKey key)
        {
            return (T)_componentContext.ResolveKeyed(key, typeof(T));
        }

        private T ResolveDefaultKeyService()
        {
            return (T)_componentContext.ResolveKeyed(DefaultKeyRegistrationType.Default, typeof(T));
        }

        private T ResolveComponentAttributeService(TKey key)
        {
            if (_registrations.Value.ContainsKey(key))
            {
                return (T)_componentContext.ResolveComponent(_registrations.Value[key], Enumerable.Empty<Parameter>());
            }
            throw new KeyNotFoundException($"No component registered with key '{key}'" );
        }

        private T ResolveDefaultComponentAttributeService()
        {
            return (T)_componentContext.ResolveComponent(_defaultComponentAttributeRegistration, Enumerable.Empty<Parameter>());
        }

        private bool ExistsKeyService(TKey key)
        {
            return _componentContext.IsRegisteredWithKey<T>(key);
        }

        private bool ExistsDefaultKeyService()
        {
            return _componentContext.IsRegisteredWithKey<T>(DefaultKeyRegistrationType.Default);
        }

        private bool ExistsComponentAttributeService(TKey key)
        {
            return _registrations.Value.ContainsKey(key);
        }

        private bool ExistsDefaultComponentAttributeService()
        {
            return _defaultComponentAttributeRegistration != null;
        }

        private IDictionary<TKey, IComponentRegistration> CollectRegistrations()
        {
            var registrations = new Dictionary<TKey, IComponentRegistration>();

            foreach (var registration in _componentContext.ComponentRegistry.Registrations
                                                          .Where(x => x.Services.OfType<TypedService>()
                                                                       .Any(y => y.ServiceType == typeof(T))))
            {
                var attribute = registration.Activator.LimitType.GetCustomAttribute<ComponentKeyAttribute>(false);
                if (attribute != null)
                {
                    if (attribute.IsDefault)
                    {
                        if (_defaultComponentAttributeRegistration != null)
                        {
                            throw new ConfigurationException($"Two default-Components registered for Type={typeof(T)} and Key={attribute.Key}");
                        }
                        _defaultComponentAttributeRegistration = registration;
                    }
                    else
                    {
                        registrations.Add((TKey)attribute.Key, registration);
                    }
                }
            }
            return registrations;
        }

    }
}