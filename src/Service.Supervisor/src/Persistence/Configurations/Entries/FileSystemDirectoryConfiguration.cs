using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations;

internal sealed class FileSystemDirectoryConfiguration : IEntityTypeConfiguration<FileSystemDirectory>
{
    public void Configure(EntityTypeBuilder<FileSystemDirectory> builder)
    {
        builder
            .OwnsOne(p => p.PathInfo, pathinfo =>
            {
                pathinfo
                    .Property(p => p.Name)
                    .HasColumnType("citext");

                pathinfo
                    .Property(p => p.FullName)
                    .HasColumnType("citext");

                pathinfo
                    .HasIndex(p => p.FullName)
                    .IsUnique();

                pathinfo
                    .Property(p => p.DirectoryPath)
                    .HasColumnType("citext");

                pathinfo
                    .Property(p => p.Container)
                    .HasConversion(
                        value => value == null ? null : value.Extension,
                        value => value == null
                            ? null
                            : Enumeration.Parse<VideoFileContainer>(p => p.Extension == value));

                pathinfo
                    .HasIndex(p => p.FullNameNormalized)
                    .HasMethod("gist");

                pathinfo
                    .Property(p => p.FullNameNormalized)
                    .HasColumnType("ltree");

                // https://github.com/dotnet/efcore/issues/18529
                pathinfo
                    .Property<byte[]>(nameof(IHasConcurrencyToken.ConcurrencyToken))
                    .IsRowVersion()
                    .HasColumnName("concurrency_token");
            });
    }
}