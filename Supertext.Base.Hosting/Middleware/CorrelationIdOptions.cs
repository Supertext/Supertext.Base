namespace Supertext.Base.Hosting.Middleware;

public class CorrelationIdOptions
{
    internal const string DefaultHeader = "x-correlation-id";

    /// <summary>
    /// The header field name where the correlation ID will be stored
    /// </summary>
    public string Header => DefaultHeader;

    /// <summary>
    /// Controls whether the correlation ID is returned in the response headers
    /// </summary>
    public bool IncludeInResponse => true;
}