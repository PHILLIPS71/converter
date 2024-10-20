using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Libraries;

public sealed class LibraryConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder
            .Property(p => p.Name)
            .HasColumnType("citext");

        builder
            .Property(p => p.Name)
            .HasConversion<LowerCaseConverter>();

        builder
            .Property(p => p.Slug)
            .HasColumnType("citext");

        builder
            .Property(p => p.Slug)
            .HasConversion<LowerCaseConverter>();

        builder
            .HasIndex(p => p.Name)
            .IsUnique();

        builder
            .HasIndex(p => p.Slug)
            .IsUnique();
    }
}