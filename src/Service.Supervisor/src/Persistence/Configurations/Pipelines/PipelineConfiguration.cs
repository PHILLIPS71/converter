using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations;

internal sealed class PipelineConfiguration : IEntityTypeConfiguration<Pipeline>
{
    public void Configure(EntityTypeBuilder<Pipeline> builder)
    {
        builder
            .Property(p => p.Name)
            .HasConversion(
                name => name.Value,
                value => PipelineName.Create(value).Value);

        builder
            .Property(p => p.Name)
            .HasColumnType("citext");

        builder
            .Property(p => p.Slug)
            .HasConversion(
                slug => slug.Value,
                value => PipelineSlug.Create(value).Value);

        builder
            .Property(p => p.Slug)
            .HasColumnType("citext");

        builder
            .HasIndex(p => p.Name)
            .IsUnique();

        builder
            .HasIndex(p => p.Slug)
            .IsUnique();
    }
}