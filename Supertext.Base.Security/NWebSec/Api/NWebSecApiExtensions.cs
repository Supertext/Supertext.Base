using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Security.NWebSec.Api
{
    public static class Extensions
    {
        public static void NwebSecApiPreStaticFilesSetup(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<NWebSecConfig>();
            app.UseHsts(hsts => hsts.MaxAge(config.StrictTransportSecurityHeaderMaxAge));
            app.UseReferrerPolicy(opts => opts.NoReferrer());
        }

        public static void NwebSecApiPostStaticFilesSetup(this IApplicationBuilder app)
        {
            app.UseXfo(options => options.SameOrigin());
        }
    }
}
