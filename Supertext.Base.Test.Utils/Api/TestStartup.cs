using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Test.Utils.Api
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMvc();
        }
    }
}