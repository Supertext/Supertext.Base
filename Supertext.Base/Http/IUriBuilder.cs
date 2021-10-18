using System;

namespace Supertext.Base.Http
{
    public interface IUriBuilder
    {
        Uri CreateAbsoluteUri(string relativeUrl);

        /// <summary>
        /// To use ResolveUrl, the URL template should contain the place holder {domain}
        /// </summary>
        /// <param name="urlTemplate"></param>
        /// <returns></returns>
        Uri ResolveUrl(string urlTemplate);
    }
}