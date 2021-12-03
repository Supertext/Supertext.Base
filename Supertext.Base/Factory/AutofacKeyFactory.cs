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
        private readonly Lazy<IDictionary<TKey, ServiceRegistration>> _registrations;
        private ServiceRegistration _defaultComponentAttributeRegistration;

        public AutofacKeyFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
            _registrations = new Lazy<IDictionary<TKey, ServiceRegistration>>(CollectRegistrations);
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

        public T CreateComponent(TKey key, params Parameter[] parameters)
        {
            Validate.NotNull(key, "key");

            if (ExistsKeyService(key))
            {
                return ResolveKeyService(key, parameters);
            }
            if (ExistsDefaultKeyService())
            {
                return ResolveDefaultKeyService(parameters);
            }
            if (ExistsComponentAttributeService(key))
            {
                return ResolveComponentAttributeService(key, parameters);
            }
            if (ExistsDefaultComponentAttributeService())
            {
                return ResolveDefaultComponentAttributeService(parameters);
            }
            throw new KeyNotFoundException($"No component registered with key '{key}'");
        }

        private T ResolveKeyService(TKey key)
        {
            return (T)_componentContext.ResolveKeyed(key, typeof(T));
        }

        private T ResolveKeyService(TKey key, IEnumerable<Parameter> parameters)
        {
            return (T)_componentContext.ResolveKeyed(key, typeof(T), parameters);
        }

        private T ResolveDefaultKeyService()
        {
            return (T)_componentContext.ResolveKeyed(DefaultKeyRegistrationType.Default, typeof(T));
        }

        private T ResolveDefaultKeyService(IEnumerable<Parameter> parameters)
        {
            return (T)_componentContext.ResolveKeyed(DefaultKeyRegistrationType.Default, typeof(T), parameters);
        }

        private T ResolveComponentAttributeService(TKey key)
        {
            if (_registrations.Value.ContainsKey(key))
            {
                var resolveRequest = new ResolveRequest(new UniqueService(), _registrations.Value[key], Enumerable.Empty<Parameter>());

                return (T)_componentContext.ResolveComponent(resolveRequest);
            }
            throw new KeyNotFoundException($"No component registered with key '{key}'" );
        }

        private T ResolveComponentAttributeService(TKey key, IEnumerable<Parameter> parameters)
        {
            if (_registrations.Value.ContainsKey(key))
            {
                var resolveRequest = new ResolveRequest(new UniqueService(), _registrations.Value[key], parameters);

                return (T)_componentContext.ResolveComponent(resolveRequest);
            }
            throw new KeyNotFoundException($"No component registered with key '{key}'");
        }

        private T ResolveDefaultComponentAttributeService()
        {
            var resolveRequest = new ResolveRequest(new UniqueService(), _defaultComponentAttributeRegistration, Enumerable.Empty<Parameter>());

            return (T)_componentContext.ResolveComponent(resolveRequest);
        }

        private T ResolveDefaultComponentAttributeService(IEnumerable<Parameter> parameters)
        {
            var resolveRequest = new ResolveRequest(new UniqueService(), _defaultComponentAttributeRegistration, parameters);

            return (T)_componentContext.ResolveComponent(resolveRequest);
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
            return _defaultComponentAttributeRegistration != default;
        }

        private IDictionary<TKey, ServiceRegistration> CollectRegistrations()
        {
            var registrations = GetComponentsWithComponentKeyAttribute();
            if (registrations.Any())
            {
                return registrations;
            }

            // If nothing can be loaded, try to resolve type and recollect again.
            _componentContext.TryResolve(typeof(T), out var _);
            return GetComponentsWithComponentKeyAttribute();
        }

        private Dictionary<TKey, ServiceRegistration> GetComponentsWithComponentKeyAttribute()
        {
            var registrations = new Dictionary<TKey, ServiceRegistration>();

            foreach (var registration in _componentContext.ComponentRegistry.Registrations
                                                          .Where(x => x.Services.OfType<TypedService>()
                                                                       .Any(y => y.ServiceType == typeof(T) ||
                                                                                 typeof(T).IsAssignableFrom(y.ServiceType))))
            {
                var attribute = registration.Activator.LimitType.GetCustomAttribute<ComponentKeyAttribute>(false);
                if (attribute != null)
                {
                    if (attribute.IsDefault)
                    {
                        if (_defaultComponentAttributeRegistration != default)
                        {
                            throw new ConfigurationException($"Two default-Components registered for Type={typeof(T)} and Key={attribute.Key}");
                        }

                        _defaultComponentAttributeRegistration = new ServiceRegistration(registration.ResolvePipeline, registration);
                    }
                    else
                    {
                       registrations.Add((TKey) attribute.Key, new ServiceRegistration(registration.ResolvePipeline, registration));
                    }
                }
            }

            return registrations;
        }
    }
}