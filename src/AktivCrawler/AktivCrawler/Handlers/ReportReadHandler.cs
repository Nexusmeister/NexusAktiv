using AktivCrawler.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AktivCrawler.Handlers;

public class ReportReadHandler : INotificationHandler<ReportRead>
{
    private readonly ILogger _logger;

    public ReportReadHandler(ILogger logger)
    {
        _logger = logger;
    }

    public async Task Handle(ReportRead notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{guid} / Match is ready for processing", notification.Id);
    }
}