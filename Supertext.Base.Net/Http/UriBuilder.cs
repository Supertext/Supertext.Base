using System;
using Microsoft.AspNetCore.Http;
using Supertext.Base.Common;
using Supertext.Base.Http;

namespace Supertext.Base.Net.Http
{
    internal class UriBuilder : IUriBuilder
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UriBuilder(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }


        public Uri CreateAbsoluteUri(string relativeUrl)
        {
            return new Uri(BaseUri, relativeUrl);
        }

        public Uri BaseUri
        {
            get
            {
                Validate.NotNull(_contextAccessor.HttpContext, nameof(_contextAccessor.HttpContext));
                Validate.NotNull(_contextAccessor.HttpContext.Request, nameof(_contextAccessor.HttpContext.Request));

                var request = _contextAccessor.HttpContext.Request;

                return new Uri($"{request.Scheme}://{request.Host}");
            }
        }
    }
}