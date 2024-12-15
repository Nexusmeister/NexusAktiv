using MediatR;

namespace AktivCrawler.Messages;

public sealed class ReportCrawled : INotification
{
    public required Guid Id { get; init; }
    public required string FileCreatedPath { get; init; }
    public required int SourceSystemId { get; set; }
}