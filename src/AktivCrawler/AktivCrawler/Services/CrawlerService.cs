using AktivCrawler.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AktivCrawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly ILogger<CrawlerService> _logger;
    private readonly HttpClient _httpClient;
    private readonly CrawlerOptions _crawlerOptions;

    // Constructor injection
    public CrawlerService(
        ILogger<CrawlerService> logger,
        HttpClient httpClient,
        IOptions<CrawlerOptions> crawlerOptions)
    {
        _logger = logger;
        _httpClient = httpClient;
        _crawlerOptions = crawlerOptions.Value;
    }

    public async Task<Stream?> RequestMatchReportAsync(int matchreportId, CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync($"{matchreportId}/{_crawlerOptions.SubUriPath}", token);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            _logger.LogInformation("{matchreportId} was not found - StatusCode: {exceptionMessage}", matchreportId, e.Message);
            await Task.Delay(_crawlerOptions.DelayInMs, token);
            return null;
        }

        var responseStream = await response.Content.ReadAsStreamAsync(token);
        return responseStream;
    }
}