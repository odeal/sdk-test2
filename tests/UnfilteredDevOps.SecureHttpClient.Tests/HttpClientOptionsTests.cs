using Xunit;

namespace UnfilteredDevOps.SecureHttpClient.Tests;

public class HttpClientOptionsTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var options = new HttpClientOptions();

        Assert.Equal(string.Empty, options.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(30), options.Timeout);
        Assert.Equal(3, options.MaxRetryAttempts);
        Assert.Equal(TimeSpan.FromMilliseconds(500), options.RetryDelay);
        Assert.NotNull(options.DefaultHeaders);
        Assert.Empty(options.DefaultHeaders);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var options = new HttpClientOptions
        {
            BaseAddress = "https://api.example.com",
            Timeout = TimeSpan.FromSeconds(60),
            MaxRetryAttempts = 5,
            RetryDelay = TimeSpan.FromSeconds(2),
            DefaultHeaders = new Dictionary<string, string>
            {
                { "Authorization", "Bearer EXAMPLE_TOKEN" }
            }
        };

        Assert.Equal("https://api.example.com", options.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(60), options.Timeout);
        Assert.Equal(5, options.MaxRetryAttempts);
        Assert.Equal(TimeSpan.FromSeconds(2), options.RetryDelay);
        Assert.Single(options.DefaultHeaders);
    }
}
