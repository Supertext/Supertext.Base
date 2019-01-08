namespace Supertext.Base.IO.StreamHandling
{
    public interface IStreamFactory
    {
        IStreamReader CreateStreamReader(string path);

        IStreamWriter GetStreamWriter(string path);
    }
}