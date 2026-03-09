using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace UnfilteredDevOps.SecureHttpClient;

public class SecureHttpClient : IDisposable
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly ILogger<SecureHttpClient>? _logger;
    private readonly ResiliencePipeline<HttpResponseMessage> _retryPipeline;

    public SecureHttpClient(HttpClientOptions? options = null, ILogger<SecureHttpClient>? logger = null)
    {
        options ??= new HttpClientOptions();
        _logger = logger;

        _httpClient = new System.Net.Http.HttpClient
        {
            Timeout = options.Timeout
        };

        if (!string.IsNullOrEmpty(options.BaseAddress))
        {
            _httpClient.BaseAddress = new Uri(options.BaseAddress);
        }

        foreach (var header in options.DefaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        _retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = options.MaxRetryAttempts,
                Delay = options.RetryDelay,
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => r.StatusCode >= HttpStatusCode.InternalServerError || 
                                       r.StatusCode == HttpStatusCode.RequestTimeout),
                OnRetry = args =>
                {
                    _logger?.LogWarning(
                        "Retry attempt {Attempt} after {Delay}ms due to {Outcome}",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Result?.StatusCode ?? (object?)args.Outcome.Exception?.Message
                    );
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("GET request to {Uri}", requestUri);
        
        return await _retryPipeline.ExecuteAsync(async ct => 
            await _httpClient.GetAsync(requestUri, ct), cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("POST request to {Uri}", requestUri);
        
        return await _retryPipeline.ExecuteAsync(async ct => 
            await _httpClient.PostAsync(requestUri, content, ct), cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("PUT request to {Uri}", requestUri);
        
        return await _retryPipeline.ExecuteAsync(async ct => 
            await _httpClient.PutAsync(requestUri, content, ct), cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("DELETE request to {Uri}", requestUri);
        
        return await _retryPipeline.ExecuteAsync(async ct => 
            await _httpClient.DeleteAsync(requestUri, ct), cancellationToken);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
