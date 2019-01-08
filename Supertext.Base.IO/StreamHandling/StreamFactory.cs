namespace Supertext.Base.IO.StreamHandling
{
    internal class StreamFactory : IStreamFactory
    {
        public IStreamReader CreateStreamReader(string path)
        {
            return new StreamReaderWrapper(path);
        }

        public IStreamWriter GetStreamWriter(string path)
        {
            return new StreamWriterWrapper(path);
        }
    }
}
