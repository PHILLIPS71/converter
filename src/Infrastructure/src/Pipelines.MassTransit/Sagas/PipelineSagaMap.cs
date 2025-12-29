using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed class PipelineSagaMap : SagaClassMap<PipelineSagaState>
{
    protected override void Configure(EntityTypeBuilder<PipelineSagaState> builder, ModelBuilder model)
    {
        builder
            .HasIndex(p => p.CorrelationId)
            .IsUnique();

        builder
            .Property(p => p.Pipeline)
            .HasJsonConversion();

        builder
            .Property(p => p.Context)
            .HasJsonConversion();

        builder
            .Property(p => p.Stages)
            .HasJsonConversion();
    }
}
