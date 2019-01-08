using System;

namespace Supertext.Base.IO.StreamHandling
{
    public interface IStreamReader : IDisposable
    {
        string ReadLine();

        void Close();
    }
}