using Autofac;
using FakeItEasy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Supertext.Base.Common;
using Supertext.Base.Hosting.Extensions;

namespace Supertext.Base.Hosting.Specs.Middleware.Infrastructure
{
    public class Startup
    {
        public const string Guid = "BCEF2264-26D3-46B3-BAB7-54C40FE8D8F5";

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var guidFactory = A.Fake<ISequentialGuidFactory>();
            A.CallTo(() => guidFactory.Create()).Returns(System.Guid.Parse(Guid));

            builder.RegisterInstance(guidFactory);
            builder.RegisterModule<HostingModule>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            app.UseCorrelationId();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapControllers();
                             });
        }
    }
}