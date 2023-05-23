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

namespace Supertext.Base.Test.Mvc.MinimalApi
{

    /// <summary>
    /// Factory for bootstrapping an application in memory for functional end to end tests.
    /// </summary>
    /// <typeparam name="TEntryPoint">A type in the entry point assembly of the application.
    /// Typically the Startup or Program classes can be used.</typeparam>
    public class IntegrationTestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private readonly string _url;
        private readonly IDictionary<long, List<Claim>> _userClaims;
        private readonly List<Action<ContainerBuilder>> _mockRegistrations;
        private readonly ICollection<Action<IHost>> _postBuildActions;

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userClaims">Will be used in users ClaimsPrinciple</param>
        public IntegrationTestWebApplicationFactory(string url, IDictionary<long, List<Claim>> userClaims = null)
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

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseUrls(_url)
                   .UseEnvironment("Development")
                   .UseContentRoot(AppContext.BaseDirectory)
                   .ConfigureTestServices(services =>
                                          {
                                              services.AddAuthentication("Test")
                                                      .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                                          })
                   .ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(new TestLoggerProvider(InMemoryLogger)));
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseServiceProviderFactory(new IntegrationTestAutofacServiceProviderFactory(RegisterMockedComponents));
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
            Console.WriteLine("Registering mocks...");
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