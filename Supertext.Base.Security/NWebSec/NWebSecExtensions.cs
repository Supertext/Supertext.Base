using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NWebsec.Core.Common.HttpHeaders.Configuration;

namespace Supertext.Base.Security.NWebSec
{
    public static class NWebSecExtensions
    {
        public static void NwebSecPreStaticFilesSetup(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<NWebSecConfig>();
            app.UseHsts(hsts => hsts.MaxAge(config.StrictTransportSecurityHeaderMaxAge));
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            ConfigureContentSecurityPolicy(app);
        }

        public static void NwebSecPostStaticFilesSetup(this IApplicationBuilder app)
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
                                  .DefaultSources(GetCspConfigurationFor("default-src", parser))
                                  .ScriptSources(GetCspConfigurationFor("script-src", parser))
                                  .StyleSources(GetCspConfigurationFor("style-src", parser))
                                  .ImageSources(GetCspConfigurationFor("img-src", parser))
                                  .ObjectSources(GetCspConfigurationFor("object-src", parser))
                                  .MediaSources(GetCspConfigurationFor("media-src", parser))
                                  .FrameSources(GetCspConfigurationFor("frame-src", parser))
                                  .FontSources(GetCspConfigurationFor("font-src", parser))
                                  .ConnectSources(GetCspConfigurationFor("connect-src", parser))
                                  .FrameAncestors(GetCspConfigurationFor("frame-ancestors", parser))
                                  .BaseUris(GetCspConfigurationFor("base-uri", parser))
                                  .ChildSources(GetCspConfigurationFor("child-src", parser))
                                  .FormActions(GetCspConfigurationFor("form-action", parser))
                      );
        }

        private static Action<ICspDirectiveBasicConfiguration> GetCspConfigurationFor(string source, IHeaderConfigurationParser parser)
        {
            var input = parser.Parse(source);

            if (input.Any())
            {
                return s => s.Self().CustomSources(input.ToArray());
            }

            return s => s.Self();
        }
    }
}