using Microsoft.EntityFrameworkCore;
using NexusCrawler.Data.Models;

namespace AktivCrawler.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Season> Seasons { get; set; }
}