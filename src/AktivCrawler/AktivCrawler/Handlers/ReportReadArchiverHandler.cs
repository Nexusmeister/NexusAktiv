using AktivCrawler.Messages;
using AktivCrawler.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AktivCrawler.Handlers;

public sealed class ReportReadArchiverHandler(
    IFileManagerService fileManagerService,
    ILogger<ReportReadArchiverHandler> logger)
    : INotificationHandler<ReportRead>
{
    public Task Handle(ReportRead notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Request to archive file for process {processId}", notification.Id);
        var fileArchiveSucceeded = fileManagerService.ArchiveFile(notification.Id);

        if (!fileArchiveSucceeded)
        {
            logger.LogWarning("Archiving file was not successful for process {processId}", notification.Id);
        }

        return Task.CompletedTask;
    }
}