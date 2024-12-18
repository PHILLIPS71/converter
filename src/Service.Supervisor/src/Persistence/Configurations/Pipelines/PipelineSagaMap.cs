using Giantnodes.Service.Supervisor.Persistence.Sagas;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Pipelines;

internal sealed class PipelineSagaMap : SagaClassMap<PipelineSagaState>
{
    protected override void Configure(EntityTypeBuilder<PipelineSagaState> builder, ModelBuilder model)
    {
        builder
            .HasIndex(p => p.JobId)
            .IsUnique();

        builder
            .Property(p => p.Definition)
            .HasJsonConversion();

        builder
            .Property(p => p.Context)
            .HasJsonConversion();
    }
}