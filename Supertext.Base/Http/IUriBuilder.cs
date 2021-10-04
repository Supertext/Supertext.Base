using System;

namespace Supertext.Base.Http
{
    public interface IUriBuilder
    {
        Uri CreateAbsoluteUri(string relativeUrl);

        Uri BaseUri { get; }
    }
}