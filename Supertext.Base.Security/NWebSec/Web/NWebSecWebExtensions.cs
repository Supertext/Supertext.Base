using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NWebsec.Core.Common.HttpHeaders.Configuration;

namespace Supertext.Base.Security.NWebSec.Web
{
    public static class Extensions
    {
        public static void NwebSecWebPreStaticFilesSetup(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<NWebSecConfig>();
            app.UseHsts(hsts => hsts.MaxAge(config.StrictTransportSecurityHeaderMaxAge));
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            ConfigureContentSecurityPolicy(app);
        }

        public static void NwebSecWebPostStaticFilesSetup(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<NWebSecConfig>();
            var domainArray = config.AllowedRedirectDestinations;

            app.UseXfo(options => options.SameOrigin());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseRedirectValidation(opts =>
                                      {
                                          opts.AllowSameHostRedirectsToHttps();
                                          opts.AllowedDestinations(domainArray);
                                      });
        }

        private static void ConfigureContentSecurityPolicy(this IApplicationBuilder app)
        {
            var parser = app.ApplicationServices.GetService<IHeaderConfigurationParser>();

            app.UseCsp(options => options
                                  .DefaultSources(GetBasicCspConfigurationFor("default-src", parser))
                                  .ScriptSources(GetCspConfigurationFor("script-src", parser))
                                  .StyleSources(GetBasicCspConfigurationFor("style-src", parser))
                                  .ImageSources(GetBasicCspConfigurationFor("img-src", parser))
                                  .ObjectSources(GetBasicCspConfigurationFor("object-src", parser))
                                  .MediaSources(GetBasicCspConfigurationFor("media-src", parser))
                                  .FrameSources(GetBasicCspConfigurationFor("frame-src", parser))
                                  .FontSources(GetBasicCspConfigurationFor("font-src", parser))
                                  .ManifestSources(GetBasicCspConfigurationFor("manifest-src", parser))
                                  .ConnectSources(GetBasicCspConfigurationFor("connect-src", parser))
                                  .FrameAncestors(GetBasicCspConfigurationFor("frame-ancestors", parser))
                                  .BaseUris(GetBasicCspConfigurationFor("base-uri", parser))
                                  .ChildSources(GetBasicCspConfigurationFor("child-src", parser))
                                  .FormActions(GetBasicCspConfigurationFor("form-action", parser))
                      );
        }

        private static Action<ICspDirectiveBasicConfiguration> GetBasicCspConfigurationFor(string source, IHeaderConfigurationParser parser)
        {
            var input = parser.Parse(source).ToList();

            if (input.Any())
            {
                return s => s.Self().CustomSources(input.ToArray());
            }

            return s => s.Self();
        }

        private static Action<ICspDirectiveConfiguration> GetCspConfigurationFor(string source, IHeaderConfigurationParser parser)
        {
            var input = parser.Parse(source).ToList();

            if (input.Any())
            {
                return s => s.Self().CustomSources(input.ToArray()).UnsafeInline();
            }

            return s => s.Self().UnsafeInline();
        }
    }
}