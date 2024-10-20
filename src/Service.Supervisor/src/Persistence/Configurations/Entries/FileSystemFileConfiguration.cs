using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Entries;

public sealed class FileSystemFileConfiguration : IEntityTypeConfiguration<FileSystemFile>
{
    public void Configure(EntityTypeBuilder<FileSystemFile> builder)
    {
        builder
            .OwnsOne(p => p.PathInfo, pathinfo =>
            {
                // https://github.com/dotnet/efcore/issues/18529
                pathinfo
                    .Property<byte[]>(nameof(IHasConcurrencyToken.ConcurrencyToken))
                    .IsRowVersion()
                    .HasColumnName("concurrency_token");
            });
    }
}