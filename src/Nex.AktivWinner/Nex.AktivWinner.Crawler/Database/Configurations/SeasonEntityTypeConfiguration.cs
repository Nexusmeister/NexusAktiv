using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nex.AktivWinner.Data.Models;

namespace Nex.AktivWinner.Crawler.Database.Configurations;

public class SeasonEntityTypeConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.HasKey(e => e.Id)
            .HasName("PK_Seasons")
            .IsClustered();

        builder.Property(e => e.Id)
            .UseIdentityColumn()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.DateFrom)
            .IsRequired();

        builder.Property(e => e.DateTo)
            .IsRequired();

        builder.HasMany(s => s.Leagues)
            .WithOne(l => l.Season)
            .HasForeignKey(fk => fk.SeasonId);
    }
}