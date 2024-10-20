using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Entries;

public sealed class FileSystemEntryConfiguration : IEntityTypeConfiguration<FileSystemEntry>
{
    public void Configure(EntityTypeBuilder<FileSystemEntry> builder)
    {
        builder
            .UseTpcMappingStrategy();
    }
}