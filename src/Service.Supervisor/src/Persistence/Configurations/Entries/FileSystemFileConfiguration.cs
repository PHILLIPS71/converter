using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Enumerations;
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
            });

        builder
            .OwnsMany(p => p.VideoStreams, stream =>
            {
                stream
                    .Property<Guid>("id")
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<NewIdValueGenerator>();

                stream
                    .OwnsOne(p => p.Quality, quality =>
                    {
                        quality
                            .Property(p => p.Resolution)
                            .HasConversion(
                                value => value.Id,
                                value => Enumeration.Parse<VideoResolution>(p => p.Id == value));
                    });
            });

        builder
            .OwnsMany(p => p.AudioStreams, stream =>
            {
                stream
                    .Property<Guid>("id")
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<NewIdValueGenerator>();
            });

        builder
            .OwnsMany(p => p.SubtitleStreams, stream =>
            {
                stream
                    .Property<Guid>("id")
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<NewIdValueGenerator>();
            });
    }
}