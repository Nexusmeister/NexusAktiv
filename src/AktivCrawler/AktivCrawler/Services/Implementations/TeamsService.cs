using AktivCrawler.Database;
using AktivCrawler.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NexusCrawler.Domain.Models;

namespace AktivCrawler.Services.Implementations;

public class TeamsService(IDbContextFactory<AppDbContext> dbContextFactory) : ITeamsService
{
    public async Task<Team?> GetTeamByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var team = await dbContext.Teams.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(name), cancellationToken);

        return team;
    }

    public async Task<Team> InsertTeamAsync(string name, int? clubId, CancellationToken cancellationToken = default)
    {
        var team = await GetTeamByNameAsync(name, cancellationToken);

        if (team is not null)
        {
            return team;
        }

        team = new Team
        {
            Name = name
        };
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.Teams.Add(team);

        throw new NotImplementedException();

    }
}