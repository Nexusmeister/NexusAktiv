using AktivCrawler.Messages;
using AktivCrawler.Services;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AktivCrawler.Workers;

public class ReaderWorker : BackgroundService, INotificationHandler<ReportCrawled>
{
    private readonly ILogger<ReaderWorker> _logger;
    private readonly IReaderService _readerService;
    private readonly ITextToEntitiesService _analyzerService;
    private List<ReportCrawled> _todo;

    public ReaderWorker(ILogger<ReaderWorker> logger, IReaderService readerService, ITextToEntitiesService analyzerService)
    {
        _logger = logger;
        _readerService = readerService;
        _analyzerService = analyzerService;
        _todo = new List<ReportCrawled>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var reportCrawled in _todo)
            {
                _logger.LogInformation("Start processing {guid}", reportCrawled.Id);

                var pdfcontent = _readerService.ReadData(reportCrawled.FileCreatedPath);
                var readerResult = _analyzerService.GetEntitiesFromText(pdfcontent);

                // TODO Publish Messages:
                /*
                 * 1. Metadata
                 * 2. Clubs found
                 * 3. Match results
                 */

                _todo.Remove(reportCrawled);
            }
        }
    }

    public Task Handle(ReportCrawled notification, CancellationToken cancellationToken)
    {
        _todo.Add(notification);
        _logger.LogInformation("{guid} / {filename} added to queue for read process", notification.Id, notification.FileCreatedPath);
        return Task.CompletedTask;
    }
}