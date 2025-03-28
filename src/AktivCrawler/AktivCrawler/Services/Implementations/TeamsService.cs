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

    public Task<Team> InsertTeamAsync(string name, int? clubId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}