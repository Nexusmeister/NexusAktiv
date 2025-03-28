using AktivCrawler.Database;
using AktivCrawler.Entities;
using AktivCrawler.Messages;
using AktivCrawler.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AktivCrawler.Handlers;

public class ReportReadHandler(IDbContextFactory<AppDbContext> dbContextFac,
    ISeasonsService seasonsService,
    ILogger<ReportReadHandler> logger) : INotificationHandler<ReportRead>
{
    public async Task Handle(ReportRead notification, CancellationToken cancellationToken)
    {
        var dbContext = await dbContextFac.CreateDbContextAsync(cancellationToken);
        logger.LogInformation("{guid} / Match is ready for processing", notification.Id);
        logger.LogDebug("Check for existence of season");

        // Lines of specific information are static and don't change!
        var metadata = notification.MatchInformation.Where(x => x is MetadataReaderResult { ReadLines.Length: > 0 }).ToList();

        var matchdateStrings = metadata
            .SelectMany(x => x.ReadLines ?? [])
            .Where(y => DateOnly.TryParse(y, out _))
            .ToList();

        if (matchdateStrings.Count > 1)
        {
            logger.LogWarning("Multiple Dates for {guid} found - cannot determine match date. Abort process.", notification.Id);
            return;
        }

        DateOnly.TryParse(matchdateStrings.FirstOrDefault(), out var matchdate);

        var seasonForMatch = await dbContext.Seasons.FirstOrDefaultAsync(x => x.DateFrom <= matchdate && x.DateTo >= matchdate, cancellationToken);

        if (seasonForMatch is null)
        {
            logger.LogWarning("No season found for {guid}. Try adding new season", notification.Id);
            seasonForMatch = await seasonsService.InsertSeasonForMatchdate(matchdate, cancellationToken);
        }

        var dt = new List<string>();
        metadata.ForEach(x =>
        {
            if (x.ReadLines != null)
            {
                dt.AddRange(x.ReadLines);
            }
        });

        logger.LogDebug("{guid}: Read following metadata {metadata}", notification.Id, string.Join(Environment.NewLine, dt));

        //var matchBegin = new DateTime(DateOnly.Parse(dt[16]), TimeOnly.Parse(dt[22]));
        //var location = dt[18];

        //var league = dt[16]; // TODO Check existence of league
        //var homeTeam = dt[27]; // TODO Check existence of club + team
        //var awayTeam = dt[28]; // TODO Check existence of club + team
    }
}