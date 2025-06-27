namespace Nex.AktivWinner.Crawler.Services;

public interface ICrawlerService
{
    Task<Stream?> RequestMatchReportAsync(int matchreportId, CancellationToken token = default);
}