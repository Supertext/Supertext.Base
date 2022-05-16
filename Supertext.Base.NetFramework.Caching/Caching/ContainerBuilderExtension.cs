using System;
using Autofac;
using Supertext.Base.Caching;
using Supertext.Base.Caching.Caching;
using Supertext.Base.Common;

namespace Supertext.Base.NetFramework.Caching.Caching
{
    public static class ContainerBuilderExtension
    {
        [Obsolete("Deprecated, rather use the library Supertext.Base.Caching")]
        public static void RegisterCache<TCachingType, TSettingsType>(this ContainerBuilder builder, string cacheName)
            where TCachingType : class
            where TSettingsType : class, ICacheSettings
        {
            builder.Register(context =>
                             {
                                 var memoryCacheName = cacheName;
                                 return new MemoryCache<TCachingType>(memoryCacheName, context.Resolve<TSettingsType>(), context.Resolve<IDateTimeProvider>());
                             })
                   .As<IMemoryCache<TCachingType>>()
                   .SingleInstance();
        }
    }
}