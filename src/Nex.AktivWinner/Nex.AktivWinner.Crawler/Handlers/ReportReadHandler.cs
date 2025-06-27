using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nex.AktivWinner.Crawler.Database;
using Nex.AktivWinner.Crawler.Entities;
using Nex.AktivWinner.Crawler.Messages;
using Nex.AktivWinner.Crawler.Services;
using Nex.AktivWinner.Crawler.Services.Interfaces;

namespace Nex.AktivWinner.Crawler.Handlers;

public class ReportReadHandler(IDbContextFactory<AppDbContext> dbContextFac,
    ISeasonsService seasonsService,
    ILeaguesService leaguesService,
    ITeamsService teamsService,
    ILogger<ReportReadHandler> logger) : INotificationHandler<ReportRead>
{
    public async Task Handle(ReportRead notification, CancellationToken cancellationToken)
    {
        var dbContext = await dbContextFac.CreateDbContextAsync(cancellationToken);
        logger.LogInformation("{guid} / Match is ready for processing", notification.Id);
        logger.LogDebug("Check for existence of season");

        // Lines of specific information are static and don't change!
        var metadata = notification.MatchInformation.Where(x => x is MetadataReaderResult { ReadLines.Length: > 0 }).ToList();

        // Check for matchdate to determine the season
        // For later analysis of the data we need to determine the season
        // It gives us the relation between matches and a good timeframe
        var matchdateStrings = metadata
            .SelectMany(x => x.ReadLines ?? [])
            .Where(y => DateOnly.TryParse(y, out _))
            .ToList();

        // That should not be the case, but if there are multiple dates is this match temporarily doomed
        if (matchdateStrings.Count > 1)
        {
            logger.LogWarning("Multiple Dates for {guid} found - cannot determine match date. Abort process.", notification.Id);
            return;
        }

        DateOnly.TryParse(matchdateStrings.FirstOrDefault(), out var matchdate);

        var seasonForMatch = await dbContext.Seasons.FirstOrDefaultAsync(x => x.DateFrom <= matchdate && x.DateTo >= matchdate, cancellationToken);

        // If the season in question is not existing, should we create the season to continue
        if (seasonForMatch is null)
        {
            logger.LogWarning("No season found for {guid}. Try adding new season", notification.Id);
            seasonForMatch = await seasonsService.InsertSeasonForMatchdate(matchdate, cancellationToken);
        }

        // Next is the league check
        var leagueName = metadata
            .SelectMany(x => x.ReadLines ?? []).ToList()
            .SkipWhile(x => !x.Contains("Liga/Klasse"))
            .Skip(1)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(leagueName))
        {
            logger.LogWarning("Leaguename for {guid} was empty and cannot be determined", notification.Id);
        }
        else
        {
            var league = await leaguesService.GetLeagueByNameAndMatchdateAsync(leagueName, seasonForMatch, cancellationToken);

            if (league is null)
            {
                logger.LogWarning("League {leagueName} cannot be found for season {seasonName}. Try adding it.", leagueName, seasonForMatch.Name);

                league = await leaguesService.InsertLeagueForSeasonAsync(leagueName, seasonForMatch, cancellationToken);
            }
        }

        var teamNames = metadata
            .SelectMany(x => x.ReadLines ?? [])
            .ToList()
            .TakeLast(2)
            .ToList();


        foreach (var teamName in teamNames)
        {
            var team = await teamsService.GetTeamByNameAsync(teamName, cancellationToken);

            if (team is null)
            {
                logger.LogWarning("Team {teamName} cannot be found. Try adding it.", teamName);

                team = await teamsService.InsertTeamAsync(teamName, null, cancellationToken);
            }
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