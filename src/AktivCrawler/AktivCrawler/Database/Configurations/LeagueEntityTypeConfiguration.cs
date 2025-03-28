using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCrawler.Domain.Models;

namespace AktivCrawler.Database.Configurations;

public class LeagueEntityTypeConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        builder.HasKey(e => e.Id)
            .HasName("PK_Leagues")
            .IsClustered();

        builder.Property(e => e.Id)
            .UseIdentityColumn()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasMaxLength(75)
            .IsRequired();

        builder.Property(e => e.SeasonId)
            .IsRequired();

        builder.HasOne(l => l.Season)
            .WithMany(s => s.Leagues)
            .HasForeignKey(fk => fk.SeasonId);
    }
}