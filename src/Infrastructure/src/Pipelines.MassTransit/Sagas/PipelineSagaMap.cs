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
            .Property(p => p.Pipeline)
            .HasJsonConversion();

        builder
            .Property(p => p.Context)
            .HasJsonConversion();

        builder
            .Property(p => p.Pending)
            .HasJsonConversion();

        builder
            .Property(p => p.Executing)
            .HasJsonConversion();
    }
}