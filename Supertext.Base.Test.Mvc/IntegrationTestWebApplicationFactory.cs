using System;
using System.Collections.Generic;
using System.Security.Claims;
using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supertext.Base.Extensions;
using Supertext.Base.Test.Utils.Logging;

namespace Supertext.Base.Test.Mvc
{

    /// <summary>
    /// Factory for bootstrapping an application in memory for functional end to end tests.
    /// </summary>
    /// <typeparam name="TStartup">The Startup class of the API to be tested.</typeparam>
    /// <typeparam name="TEntryPoint">A type in the entry point assembly of the application.
    /// Typically the Startup or Program classes can be used.</typeparam>
    public class IntegrationTestWebApplicationFactory<TStartup, TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TStartup : class
        where TEntryPoint : class
    {
        private readonly string _url;
        private readonly IDictionary<long, IList<Claim>> _userClaims;
        private readonly List<Action<ContainerBuilder>> _mockRegistrations;
        private readonly ICollection<Action<IHost>> _postBuildActions;

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userClaims">Will be used in users ClaimsPrinciple</param>
        public IntegrationTestWebApplicationFactory(string url, IDictionary<long, IList<Claim>> userClaims = null)
        {
            _url = url;
            _userClaims = userClaims;
            _mockRegistrations = new List<Action<ContainerBuilder>>();
            _postBuildActions = new List<Action<IHost>>();
            InMemoryLogger = new InMemoryLogger();
            RegisterComponentForDiOverwrite(builder => builder.RegisterModule<MvcModule>());
        }

        public InMemoryLogger InMemoryLogger { get; }

        public void ConfigureHostPostBuildAction(Action<IHost> postBuildAction)
        {
            _postBuildActions.Add(postBuildAction);
        }

        public void RegisterComponentForDiOverwrite(Action<ContainerBuilder> containerBuilderDelegate)
        {
            _mockRegistrations.Add(containerBuilderDelegate);
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                              .UseServiceProviderFactory(new IntegrationTestAutofacServiceProviderFactory(RegisterMockedComponents))
                              .ConfigureWebHostDefaults(host =>
                                                        {
                                                            host.UseUrls(_url)
                                                                .UseEnvironment("Development")
                                                                .UseContentRoot(AppContext.BaseDirectory)
                                                                .UseStartup<TStartup>()
                                                                .ConfigureTestServices(services =>
                                                                                       {
                                                                                           services.AddAuthentication("Test")
                                                                                                   .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                                                                                       })
                                                                .ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(new TestLoggerProvider(InMemoryLogger)));
                                                        })
                              .ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(new TestLoggerProvider(InMemoryLogger)));

            return builder;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);
            var testSettings = host.Services.GetRequiredService<TestSettings>();
            _userClaims?.ForEach(userClaims =>
            {
                var userId = userClaims.Key;
                userClaims.Value.ForEach(userClaim => testSettings.AddClaim(userId, userClaim));
            });

            ExecutePostCreateActions(host);

            return host;
        }

        private void RegisterMockedComponents(ContainerBuilder containerBuilder)
        {
            foreach (var mockRegistration in _mockRegistrations)
            {
                mockRegistration(containerBuilder);
            }
        }

        private void ExecutePostCreateActions(IHost host)
        {
            foreach (var postBuildAction in _postBuildActions)
            {
                postBuildAction(host);
            }
        }
    }
}