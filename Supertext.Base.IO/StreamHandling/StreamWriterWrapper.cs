using System.IO;

namespace Supertext.Base.IO.StreamHandling
{
    internal class StreamWriterWrapper : IStreamWriter
    {
        private readonly StreamWriter _streamWriter;

        public StreamWriterWrapper(string path)
        {
            _streamWriter = new StreamWriter(path);
        }

        public void Write(string value)
        {
            _streamWriter.Write(value);
        }

        public void WriteLine()
        {
            _streamWriter.WriteLine();
        }

        public void WriteLine(string value)
        {
            _streamWriter.WriteLine(value);
        }

        public void Close()
        {
            _streamWriter.Close();
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }
    }
}
