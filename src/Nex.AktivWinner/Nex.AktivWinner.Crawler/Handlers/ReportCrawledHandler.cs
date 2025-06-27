using MediatR;
using Microsoft.Extensions.Logging;
using Nex.AktivWinner.Crawler.Messages;
using Nex.AktivWinner.Crawler.Services;

namespace Nex.AktivWinner.Crawler.Handlers;

public class ReportCrawledHandler : INotificationHandler<ReportCrawled>
{
    private readonly ILogger<ReportCrawledHandler> _logger;
    private readonly IFileReaderService _readerService;
    private readonly ITextToEntitiesService _analyzerService;
    private readonly IMediator _mediator;

    public ReportCrawledHandler(ILogger<ReportCrawledHandler> logger, IFileReaderService readerService, ITextToEntitiesService analyzerService, IMediator mediator)
    {
        _logger = logger;
        _readerService = readerService;
        _analyzerService = analyzerService;
        _mediator = mediator;
    }

    public async Task Handle(ReportCrawled notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{guid} is ready for processing", notification.Id);
        _logger.LogInformation("Start processing {guid}", notification.Id);

        var readerResult = _analyzerService.GetEntitiesFromText(notification.ReportContent);

        _logger.LogDebug("Report {guid} got read from file", notification.Id);
        await _mediator.Publish(new ReportRead
        {
            Id = notification.Id,
            SourceSystemId = notification.SourceSystemId,
            MatchInformation = readerResult
        }, cancellationToken);
    }
}