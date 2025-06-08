using Giantnodes.Infrastructure.EntityFrameworkCore;
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
            .OwnsMany(p => p.Stages, stage =>
            {
                stage
                    .Property<Guid>("id")
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<NewIdValueGenerator>();

                stage
                    .HasIndex(p => p.JobId)
                    .IsUnique();

                stage
                    .Property(p => p.Stage)
                    .HasJsonConversion();
            });
    }
}