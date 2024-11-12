using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Entries;

public sealed class FileSystemDirectoryConfiguration : IEntityTypeConfiguration<FileSystemDirectory>
{
    public void Configure(EntityTypeBuilder<FileSystemDirectory> builder)
    {
        builder
            .OwnsOne(p => p.PathInfo, pathinfo =>
            {
                // https://github.com/dotnet/efcore/issues/18529
                pathinfo
                    .Property<byte[]>(nameof(IHasConcurrencyToken.ConcurrencyToken))
                    .IsRowVersion()
                    .HasColumnName("concurrency_token");

                pathinfo
                    .HasIndex(p => p.FullName)
                    .IsUnique();
            });
    }
}