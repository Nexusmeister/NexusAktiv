namespace AktivCrawler.Services;

public interface ICrawlerService
{
    Task<Stream?> RequestMatchReportAsync(int matchreportId, CancellationToken token = default);
}