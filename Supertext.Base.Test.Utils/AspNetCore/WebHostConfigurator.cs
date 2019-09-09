using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Test.Utils.AspNetCore
{
    internal class WebHostConfigurator : IWebHostConfigurator
    {
        private IWebHostBuilder _builder;

        public WebHostConfigurator(string[] args)
        {
           _builder = WebHost.CreateDefaultBuilder(args);
        }

        public IWebHostConfigurator UseStartup<TStartup>() where TStartup : class
        {
            _builder = _builder.UseStartup<TStartup>();
            return this;
        }

        public IWebHostConfigurator UseEnvironment(string environmentName = "Development")
        {
            _builder = _builder.UseEnvironment(environmentName);
            return this;
        }

        public IWebHostConfigurator ConfigureLogging(ILoggerProvider loggerProvider)
        {
            _builder = _builder.ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(loggerProvider));
            return this;
        }

        public IWebHostConfigurator UseUrls(params string[] urls)
        {
            _builder = _builder.UseUrls(urls);
            return this;
        }

        public IWebHostConfigurator UseHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _builder = _builder.ConfigureServices(services => services.AddTransient(svc => httpContextAccessor));
            return this;
        }

        public IWebHostConfigurator UseSetting(string key, string value)
        {
            _builder = _builder.UseSetting(key, value);
            return this;
        }

        public IWebHostConfigurator RegisterType<TService>(Func<TService> implementationFactory) where TService : class
        {
            _builder = _builder.ConfigureServices(service => service.AddTransient(serviceProvider => implementationFactory()));
            return this;
        }

        public IWebHost Build()
        {
            return _builder.Build();
        }
    }
}