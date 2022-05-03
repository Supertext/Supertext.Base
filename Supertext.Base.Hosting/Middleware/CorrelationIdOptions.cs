namespace Supertext.Base.Hosting.Middleware;

public class CorrelationIdOptions
{
    private const string DefaultHeader = "x-correlation-id";

    /// <summary>
    /// The header field name where the correlation ID will be stored
    /// </summary>
    public string Header { get; set; } = DefaultHeader;

    /// <summary>
    /// Controls whether the correlation ID is returned in the response headers
    /// </summary>
    public bool IncludeInResponse { get; set; } = true;
}