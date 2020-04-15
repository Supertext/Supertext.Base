using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.EntityFrameworkCore.Migration
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TDbContext>(this IHost host) where TDbContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<TDbContext>())
                {
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<TDbContext>>();
                        logger.LogError(ex, "An error has occurred while migrating the database.");
                        throw;
                    }
                }
            }
            return host;
        }
    }
}