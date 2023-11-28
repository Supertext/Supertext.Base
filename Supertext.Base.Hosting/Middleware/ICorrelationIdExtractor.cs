namespace Supertext.Base.Hosting.Middleware
{
    public interface ICorrelationIdExtractor
    {
        string Extract(object logEventProperty);
    }
}