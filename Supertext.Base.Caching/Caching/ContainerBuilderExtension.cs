using Autofac;
using Supertext.Base.Common;

namespace Supertext.Base.Caching.Caching
{
    public static class ContainerBuilderExtension
    {
        public static void RegisterCache<TCachingType, TSettingsType>(this ContainerBuilder builder, string cacheName) where TCachingType : class
                                                                                                                       where TSettingsType : class, ICacheSettings
        {
            builder.Register(context => new MemoryCache<TCachingType>(cacheName,
                                                                      context.Resolve<TSettingsType>(),
                                                                      context.Resolve<IDateTimeProvider>()))
                   .As<IMemoryCache<TCachingType>>()
                   .SingleInstance();
        }
    }
}