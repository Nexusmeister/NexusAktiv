using MediatR;

namespace AktivCrawler.Messages;

public class ReportCrawled : INotification
{
    public required Guid Id { get; set; }
    public required string FileCreatedPath { get; set; }
}