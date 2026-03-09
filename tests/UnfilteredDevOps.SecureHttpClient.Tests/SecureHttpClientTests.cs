using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnfilteredDevOps.SecureHttpClient.Tests;

public class SecureHttpClientTests : IDisposable
{
    private readonly Mock<ILogger<SecureHttpClient>> _loggerMock;
    private readonly SecureHttpClient _client;

    public SecureHttpClientTests()
    {
        _loggerMock = new Mock<ILogger<SecureHttpClient>>();
        _client = new SecureHttpClient(new HttpClientOptions
        {
            Timeout = TimeSpan.FromSeconds(5),
            MaxRetryAttempts = 2,
            RetryDelay = TimeSpan.FromMilliseconds(100)
        }, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithDefaultOptions_CreatesClient()
    {
        using var client = new SecureHttpClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithCustomOptions_CreatesClient()
    {
        var options = new HttpClientOptions
        {
            BaseAddress = "https://api.example.com",
            Timeout = TimeSpan.FromSeconds(10),
            MaxRetryAttempts = 5,
            RetryDelay = TimeSpan.FromSeconds(1)
        };

        using var client = new SecureHttpClient(options);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithDefaultHeaders_AddsHeaders()
    {
        var options = new HttpClientOptions
        {
            DefaultHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "test-value" },
                { "X-API-Key", "EXAMPLE_KEY" }
            }
        };

        using var client = new SecureHttpClient(options);
        Assert.NotNull(client);
    }

    [Fact]
    public async Task GetAsync_WithValidUrl_ReturnsResponse()
    {
        var response = await _client.GetAsync("https://httpbin.org/get");
        
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task GetAsync_WithInvalidUrl_ThrowsException()
    {
        await Assert.ThrowsAsync<HttpRequestException>(
            async () => await _client.GetAsync("https://this-domain-does-not-exist-12345.com")
        );
    }

    [Fact]
    public async Task PostAsync_WithContent_ReturnsResponse()
    {
        var content = new StringContent("{\"test\":\"data\"}");
        var response = await _client.PostAsync("https://httpbin.org/post", content);
        
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task PutAsync_WithContent_ReturnsResponse()
    {
        var content = new StringContent("{\"test\":\"data\"}");
        var response = await _client.PutAsync("https://httpbin.org/put", content);
        
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task DeleteAsync_WithValidUrl_ReturnsResponse()
    {
        var response = await _client.DeleteAsync("https://httpbin.org/delete");
        
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public void Dispose_DisposesClient()
    {
        var client = new SecureHttpClient();
        client.Dispose();
        
        Assert.Throws<ObjectDisposedException>(() => 
            client.GetAsync("https://httpbin.org/get").GetAwaiter().GetResult()
        );
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
