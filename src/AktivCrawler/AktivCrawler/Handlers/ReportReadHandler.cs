using AktivCrawler.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AktivCrawler.Handlers;

public class ReportReadHandler : INotificationHandler<ReportRead>
{
    private readonly ILogger<ReportReadHandler> _logger;

    public ReportReadHandler(ILogger<ReportReadHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ReportRead notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{guid} / Match is ready for processing", notification.Id);
    }
}