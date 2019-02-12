using System;
using Autofac.Builder;
using Supertext.Base.Factory;

namespace Supertext.Base.Modularity
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Register a Default-Component for the Type serviceType.
        /// </summary>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TStyle"></typeparam>
        /// <param name="registration"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> KeyedDefault<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Type serviceType)
        {
            return registration.Keyed(DefaultKeyRegistrationType.Default, serviceType);
        }
    }
}