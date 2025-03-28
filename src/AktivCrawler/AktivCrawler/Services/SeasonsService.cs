using AktivCrawler.Database;
using Microsoft.EntityFrameworkCore;
using NexusCrawler.Domain.Models;

namespace AktivCrawler.Services;

public sealed class SeasonsService(IDbContextFactory<AppDbContext> dbContextFactory) : ISeasonsService
{
    public async Task<Season?> InsertSeasonForMatchdate(DateOnly refDate, CancellationToken cancellationToken = default)
    {
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var seasonForMatch = await dbContext.Seasons.FirstOrDefaultAsync(x => x.DateFrom <= refDate && x.DateTo >= refDate, cancellationToken);

        // Already exists, we do not need an Insert
        if (seasonForMatch is not null)
        {
            return seasonForMatch;
        }

        // A season ranges from last day of June to 01.07.
        var yearOfMatch = refDate.Year;
        var monthOfMatch = refDate.Month;

        var seasonToInsert = new Season();

        // Match is in a season that ends this year
        if (monthOfMatch <= 6)
        {
            seasonToInsert.DateFrom = new DateOnly(yearOfMatch - 1, 7, 1);
            var daysInJune = DateTime.DaysInMonth(yearOfMatch - 1, 6);
            seasonToInsert.DateTo = new DateOnly(yearOfMatch, 6, daysInJune);
            seasonToInsert.Name = $"{yearOfMatch - 1}/{yearOfMatch}";
        }
        // Month >= 7 means that the match is in a season that has started this year
        else
        {
            seasonToInsert.DateFrom = new DateOnly(yearOfMatch, 7, 1);
            var daysInJune = DateTime.DaysInMonth(yearOfMatch + 1, 6);
            seasonToInsert.DateTo = new DateOnly(yearOfMatch + 1, 6, daysInJune);
            seasonToInsert.Name = $"{yearOfMatch}/{yearOfMatch + 1}";
        }

        dbContext.Add(seasonToInsert);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        // Return null to signal failing of the add operation
        return result > 0 ? seasonToInsert : null;
    }
}