using MediatR;
using Microsoft.Extensions.Logging;
using Nex.AktivWinner.Crawler.Messages;
using Nex.AktivWinner.Crawler.Services;

namespace Nex.AktivWinner.Crawler.Handlers;

public class ReportCrawledHandler(
    ILogger<ReportCrawledHandler> logger,
    ITextToEntitiesService analyzerService,
    IMediator mediator)
    : INotificationHandler<ReportCrawled>
{
    public async Task Handle(ReportCrawled notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start processing {guid}", notification.Id);

        var readerResult = analyzerService.GetEntitiesFromText(notification.ReportContent);

        logger.LogDebug("Entities from Report {guid} got determined", notification.Id);
        await mediator.Publish(new ReportRead
        {
            Id = notification.Id,
            SourceSystemId = notification.SourceSystemId,
            MatchInformation = readerResult
        }, cancellationToken);
    }
}