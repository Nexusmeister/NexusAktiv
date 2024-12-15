using AktivCrawler.Messages;
using AktivCrawler.Services;
using AktivCrawler.Workers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AktivCrawler.Handlers;

public class ReportCrawledHandler : INotificationHandler<ReportCrawled>
{
    private readonly ILogger _logger;
    private readonly IFileReaderService _readerService;
    private readonly ITextToEntitiesService _analyzerService;
    private readonly IMediator _mediator;

    public ReportCrawledHandler(ILogger logger, IFileReaderService readerService, ITextToEntitiesService analyzerService, IMediator mediator)
    {
        _logger = logger;
        _readerService = readerService;
        _analyzerService = analyzerService;
        _mediator = mediator;
    }

    public async Task Handle(ReportCrawled notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{guid} / {filename} is ready for processing", notification.Id, notification.FileCreatedPath);
        _logger.LogInformation("Start processing {guid}", notification.Id);

        var pdfcontent = _readerService.ReadData(notification.FileCreatedPath);
        var readerResult = _analyzerService.GetEntitiesFromText(pdfcontent);

        _logger.LogDebug("Report {guid} got read from file", notification.Id);
        await _mediator.Publish(new ReportRead
        {
            Id = notification.Id,
            SourceSystemId = notification.SourceSystemId,
            MatchInformation = readerResult
        }, cancellationToken);
    }
}