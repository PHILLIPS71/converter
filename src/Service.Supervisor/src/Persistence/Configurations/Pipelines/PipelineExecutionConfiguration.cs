using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Pipelines;

internal sealed class PipelineExecutionConfiguration : IEntityTypeConfiguration<PipelineExecution>
{
    public void Configure(EntityTypeBuilder<PipelineExecution> builder)
    {
        builder
            .Property(p => p.Definition)
            .HasJsonConversion();

        builder
            .OwnsOne(p => p.Failure, failure =>
            {
                failure
                    .Property(p => p.Reason)
                    .HasColumnType("citext");
            });
    }
}