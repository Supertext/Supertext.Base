using System;
using System.Collections.Generic;
using System.Security.Claims;
using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
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
    public class IntegrationTestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly IDictionary<long, List<Claim>> _userClaims;
        private readonly List<Action<ContainerBuilder>> _mockRegistrations = new();
        private readonly ICollection<Action<IHost>> _postBuildActions;

        /// <summary>
        ///
        /// </summary>
        /// <param name="userClaims">Will be used in users ClaimsPrinciple</param>
        public IntegrationTestWebApplicationFactory(IDictionary<long, List<Claim>> userClaims = null)
        {
            _userClaims = userClaims ?? new Dictionary<long, List<Claim>>();
            _postBuildActions = new List<Action<IHost>>();
            InMemoryLogger = new InMemoryLogger();
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
            builder.ConfigureAppConfiguration((_, configApp) =>
                                              {
                                                  configApp.AddJsonFile("appsettings.json", optional: true);
                                                  configApp.AddEnvironmentVariables();
                                              })
                   .ConfigureTestServices(services =>
                                          {
                                              services.AddAuthentication("Test")
                                                      .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                                          })
                   .ConfigureTestContainer<ContainerBuilder>(RegisterMocks)
                   .ConfigureLogging(loggingBuilder =>
                                     {
                                         loggingBuilder.AddProvider(new TestLoggerProvider(InMemoryLogger));
                                     });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureContainer<ContainerBuilder>(RegisterMocks);
            builder.ConfigureLogging(loggingBuilder =>
                                     {
                                         loggingBuilder.AddProvider(new TestLoggerProvider(InMemoryLogger));
                                     });
            var host = base.CreateHost(builder);

            var testSettings = host.Services.GetRequiredService<TestSettings>();
            _userClaims.ForEach(userClaims =>
                                {
                                    var userId = userClaims.Key;
                                    userClaims.Value.ForEach(userClaim => testSettings.AddClaim(userId, userClaim));
                                });

            ExecutePostCreateActions(host);
            return host;
        }

        private void ExecutePostCreateActions(IHost host)
        {
            foreach (var postBuildAction in _postBuildActions)
            {
                postBuildAction(host);
            }
        }

        private void RegisterMocks(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TestSettings>().AsSelf().SingleInstance();
            foreach (var mockRegistration in _mockRegistrations)
            {
                mockRegistration(containerBuilder);
            }
        }
    }
}