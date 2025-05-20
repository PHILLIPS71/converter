using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations;

internal sealed class PipelineExecutionConfiguration : IEntityTypeConfiguration<PipelineExecution>
{
    public void Configure(EntityTypeBuilder<PipelineExecution> builder)
    {
        builder
            .OwnsOne(p => p.Failure, failure =>
            {
                failure
                    .Property(p => p.Reason)
                    .HasColumnType("citext");
            });
    }
}