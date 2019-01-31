using System;

namespace Supertext.Base.IO.StreamHandling
{
    public interface IStreamWriter: IDisposable
    {
        void Write(string value);

        void WriteLine();

        void WriteLine(string value);

        void Close();
    }
}