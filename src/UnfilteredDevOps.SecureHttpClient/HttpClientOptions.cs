namespace UnfilteredDevOps.SecureHttpClient;

public class HttpClientOptions
{
    public string BaseAddress { get; set; } = string.Empty;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
}
