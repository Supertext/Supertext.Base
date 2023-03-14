using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Supertext.Base.Caching.Redis;

public static class CachingRedisExtensions
{
    /// <summary>
    /// Registers components for injecting a <see cref="IDecentralizedCache" which internally uses Redis. />
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="produceRedisConfig"></param>
    /// <returns></returns>
    public static IServiceCollection UseRedisCache(this IServiceCollection serviceCollection, Func<ConfigurationOptions, ConfigurationOptions> produceRedisConfig)
    {
        serviceCollection.AddScoped<IDecentralizedCache, RedisCache>();
        return serviceCollection.AddStackExchangeRedisCache(options =>
                                                            {
                                                                var configurationOptions = produceRedisConfig(new ConfigurationOptions());
                                                                options.ConfigurationOptions = configurationOptions;
                                                            });
    }
}