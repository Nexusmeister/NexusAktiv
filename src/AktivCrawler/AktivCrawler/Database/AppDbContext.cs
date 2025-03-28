using AktivCrawler.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using NexusCrawler.Domain.Models;

namespace AktivCrawler.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Season> Seasons { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new LeagueEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SeasonEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TeamEntityTypeConfiguration());
    }
}