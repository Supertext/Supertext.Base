using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Supertext.Base.Common;

namespace Supertext.Base.Test.Utils.AspNetCore
{
    public abstract class IntegrationTestBase : IDisposable
    {
        private IWebHost _webHost;

        protected abstract void ConfigureWebHost(IWebHostConfigurator webHostConfigurator);

        protected void StartWebHost(string[] args)
        {
            var configurator = new WebHostConfigurator(args);
            ConfigureWebHost(configurator);
            _webHost = configurator.Build();
            _webHost.RunAsync().ConfigureAwait(false);
        }

        protected void StopWebHost()
        {
            _webHost?.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected void MigrateDatabase<TDatabaseContext>() where TDatabaseContext : DbContext
        {
            ExecuteOnDbContext<TDatabaseContext>(context => context.Database.Migrate());
        }

        protected void MigrateDatabaseToSpecificVersion<TDatabaseContext, TMigration>()
            where TDatabaseContext : DbContext
            where TMigration : Microsoft.EntityFrameworkCore.Migrations.Migration
        {
            ExecuteOnDbContext<TDatabaseContext>(context => context.GetService<IMigrator>().Migrate(nameof(TMigration)));
        }

        // Resolves a desired type at the DI-Container. StartWebHost() has to be invoked first.
        protected TDesiredType Resolve<TDesiredType>()
        {
            Validate.NotNull(_webHost, nameof(_webHost));
            return _webHost.Services.GetRequiredService<TDesiredType>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposable)
        {
            if (disposable)
            {
                _webHost?.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                _webHost?.Dispose();
            }
        }

        private void ExecuteOnDbContext<TDatabaseContext>(Action<TDatabaseContext> contextAction) 
            where TDatabaseContext : DbContext
        {
            Validate.NotNull(_webHost, nameof(_webHost));
            using (var scope = _webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<TDatabaseContext>())
                {
                    try
                    {
                        contextAction(context);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<IntegrationTestBase>>();
                        logger.LogError(ex, "An error occurred while migrating the database.");
                        throw;
                    }
                }
            }
        }
    }
}