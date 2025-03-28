using AktivCrawler.Database;
using Microsoft.EntityFrameworkCore;
using NexusCrawler.Domain.Models;

namespace AktivCrawler.Services;

public class LeaguesService(IDbContextFactory<AppDbContext> dbContextFactory) : ILeaguesService
{
    public async Task<League?> GetLeagueByNameAndMatchdateAsync(string leagueName, Season refSeason,
        CancellationToken cancellationToken = default)
    {
        // We need to find the league for the given Season
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var league = await dbContext
            .Leagues
            .AsNoTracking()
            .Include(l => l.Season)
            .FirstOrDefaultAsync(l => l.Name.Equals(leagueName) 
                && l.Season.Id == refSeason.Id, cancellationToken: cancellationToken);

        return league;
    }

    public async Task<League?> InsertLeagueForSeasonAsync(string leagueName, Season refSeason, CancellationToken cancellationToken = default)
    {
        var league = await GetLeagueByNameAndMatchdateAsync(leagueName, refSeason, cancellationToken);

        if (league is not null)
        {
            return league;
        }

        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var leagueToInsert = new League
        {
            Name = leagueName,
            SeasonId = refSeason.Id
        };

        dbContext.Leagues.Add(leagueToInsert);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        return result > 0 ? leagueToInsert : null;
    }
}