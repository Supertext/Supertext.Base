using System.IO;

namespace Supertext.Base.IO.StreamHandling
{
    internal class StreamReaderWrapper : IStreamReader
    {
        private readonly StreamReader _streamReader;

        public StreamReaderWrapper(string path)
        {
            _streamReader = new StreamReader(path);
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }

        public void Close()
        {
            _streamReader.Close();
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}
