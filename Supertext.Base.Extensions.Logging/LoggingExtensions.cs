using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Extensions.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Clears all logging providers and reconfigures some. Add a configuration from the logging section from the appsettings.
        /// Main reason is removing the console logger, since it has a very poor performance and shouldn't be used in production.
        /// Source: https://weblog.west-wind.com/posts/2018/Dec/31/Dont-let-ASPNET-Core-Default-Console-Logging-Slow-your-App-down#overriding-the-default-logging-configuration
        /// Added loggers:
        /// + Debug
        /// + EventSourceLogger
        /// If running in dev environment:
        /// + Console logger
        /// </summary>
        public static void ConfigureLogging(this IServiceCollection services, IConfiguration configuration, string environmentName)
        {
            services.AddLogging(config =>
                                {
                                    // clear out default configuration
                                    config.ClearProviders();

                                    config.AddConfiguration(configuration.GetSection("Logging"));
                                    config.AddDebug();
                                    config.AddEventSourceLogger();

                                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == environmentName)
                                    {
                                        config.AddConsole();
                                    }
                                });
        }
    }
}
