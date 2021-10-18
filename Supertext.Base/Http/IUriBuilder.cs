using System;

namespace Supertext.Base.Http
{
    public interface IUriBuilder
    {
        Uri CreateAbsoluteUri(string relativeUrl);

        /// <summary>
        /// To use ResolveUrl method, URL should have place holder {domain}, 
        /// </summary>
        /// <param name="urlTemplate"></param>
        /// <returns></returns>
        Uri ResolveUrl(string urlTemplate);
    }
}