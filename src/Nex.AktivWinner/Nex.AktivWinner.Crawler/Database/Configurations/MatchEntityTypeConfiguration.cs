using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nex.AktivWinner.Data.Models;

namespace Nex.AktivWinner.Crawler.Database.Configurations;

public class MatchEntityTypeConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(k => k.Id)
            .IsClustered()
            .HasName("PK_Matches");

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();
    }
}