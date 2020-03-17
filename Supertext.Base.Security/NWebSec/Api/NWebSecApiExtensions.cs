using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Security.NWebSec.Api
{
    public static class Extensions
    {
        public static void NwebSecApiSetup(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<NWebSecConfig>();
            app.UseHsts(hsts => hsts.MaxAge(config.StrictTransportSecurityHeaderMaxAge));
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXfo(options => options.SameOrigin());
        }
    }
}
