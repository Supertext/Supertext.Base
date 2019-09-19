using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Test.Utils.AspNetCore
{
    public interface IWebHostConfigurator
    {
        /// <summary>Specify the environment to be used by the web host.</summary>
        IWebHostConfigurator UseEnvironment(string environmentName = "Development");

        // Configure custom logging provider
        IWebHostConfigurator ConfigureLogging(ILoggerProvider loggerProvider);

        /// <summary>Specify the urls the web host will listen on.</summary>
        IWebHostConfigurator UseUrls(params string[] urls);

        /// <summary>Specify the urls the web host will listen on.</summary>
        IWebHostConfigurator UseHttpContextAccessor(IHttpContextAccessor httpContextAccessor);

        /// <summary>Add or replace a setting in the configuration.</summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        IWebHostConfigurator UseSetting(string key, string value);

        // Register a custom type for dependency injection.
        IWebHostConfigurator RegisterType<TService>(Func<TService> implementationFactory) where TService : class;

        /// <summary>Specify the Startup type to be used by the web host.</summary>
        IWebHostConfigurator UseStartup<TStartup>() where TStartup : class;
    }
}