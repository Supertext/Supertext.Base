using Microsoft.AspNetCore.Hosting;

namespace Supertext.Base.Test.Utils.Api
{
    internal static class WebHostBuilderExtensions
    {

        #region Use Test Startup

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTestStartup"></typeparam>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseTestStartup<TTestStartup, TStartup>(this IWebHostBuilder self)
            where TTestStartup : class
            where TStartup : class
        {
            var applicationKey = typeof(TStartup).Assembly.FullName;

            return self
                   .UseStartup<TTestStartup>()
                   .UseSetting(WebHostDefaults.ApplicationKey, applicationKey);
        }

        #endregion

    }
}
