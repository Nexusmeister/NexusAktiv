using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nex.AktivWinner.Data.Models;

namespace Nex.AktivWinner.Crawler.Database.Configurations;

public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(e => e.Id)
            .HasName("PK_Teams")
            .IsClustered();

        builder.Property(e => e.Id)
            .UseIdentityColumn()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
    }
}